using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class JsonBiome
{
    public string biome_name;
    public string tile_name;
    public float avg_rain, avg_temp, rarity;
    public string[] decorations;
    public float[] decorationChances;
}

[System.Serializable]
public class BiomesJson
{
    public JsonBiome[] biomes;
}

public struct Biome
{
    public static Dictionary<string, Vector2[]> s_colliders = new Dictionary<string, Vector2[]>();
    public static Dictionary<string, int> s_altSortingOrders = new Dictionary<string, int>();
    public static Dictionary<string, GameObject> s_decorations = new Dictionary<string, GameObject>();

    public GameObject[] decorations;
    public float[] decorationThreshholds;

    public Biome(JsonBiome jsonBiome)
    {
        decorations = new GameObject[jsonBiome.decorations.Length];
        decorationThreshholds = new float[jsonBiome.decorations.Length];
        for(int i=0; i<decorations.Length; i++)
        {
            var name = jsonBiome.decorations[i];
            if(!s_decorations.ContainsKey(name))
            {
                GameObject go = new GameObject(name, typeof(SpriteRenderer));
                
                var sr = go.GetComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("Sprites/Decorations/"+name);
                sr.sortingOrder = s_altSortingOrders.ContainsKey(name) ? s_altSortingOrders[name] : 1;
                sr.spriteSortPoint = SpriteSortPoint.Pivot;
                
                if(s_colliders.ContainsKey(name))
                {
                    var c = go.AddComponent<PolygonCollider2D>();
                    c.pathCount = 1;
                    c.SetPath(0, s_colliders[name]);
                }

                s_decorations.Add(name, go);
                go.SetActive(false);
            }
            decorations[i] = s_decorations[name];
            decorationThreshholds[i] = jsonBiome.decorationChances[i];
            decorationThreshholds[i] += (i > 0 ? decorationThreshholds[i-1] : 0);
        }
    }
}

public class ProceduralGeneration : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_chunk;

    public static ProceduralGeneration instance;
    
    [HideInInspector]
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos;
    public const int chunkSize=50;
    public const int mapRadius=16;
    public const int mapDiameter=mapRadius*2;
    public static readonly Vector2Int center = Vector2Int.one*mapRadius*chunkSize;
    public static Dictionary<(int x, int y), GameObject> loadedChunks;
    public static LinkedList<GameObject> disabledChunks;
    public int renderDistance;
    public static bool reset=true;

    public static RuleTile[] tiles;
    public static Biome[] biomes;
    public static int rain_temp_map_width=100;
    public static byte[,] rain_temp_map;

    public static byte[,] mapTexture_biome;
    public static byte[,] mapTexture_decor;

    public static Texture2D textureBiome;
    public static Texture2D textureDecor;

    public static float seed_main; // determines land/water
    public static float seed_temp, seed_rain; // used for biome decision making
    public static int seed_decor;

    public static RuleTile LoadTile(string name)
    {
        return Resources.Load<RuleTile>("Tiles/"+name);
    }

    public void Init()
    {
        instance = this;

        var rain_temp_map_tex = Resources.Load<Texture2D>("BiomeData/rain_temp_map");
        var collidersTxt = Resources.Load<TextAsset>("DecorationData/colliders").text.Split('\n');
        Biome.s_colliders.Clear();
        for(int i=0; i<collidersTxt.Length; i++)
        {
            var line = collidersTxt[i].Split(':');
            var pointsTxt = line[1].Split(';');
            var points = new Vector2[pointsTxt.Length];
            for(int p=0; p<points.Length; p++)
            {
                var point = pointsTxt[p].Split(',');
                for(int xy=0; xy<2; xy++)
                {
                    points[p][xy] = float.Parse(point[xy]);
                }
            }
            Biome.s_colliders.Add(line[0], points);
        }

        var sortingOrdersTxt = Resources.Load<TextAsset>("DecorationData/altSortingOrders").text.Split('\n');
        Biome.s_altSortingOrders.Clear();
        for(int i=0; i<sortingOrdersTxt.Length; i++)
        {
            var line = sortingOrdersTxt[i].Split(':');
            Biome.s_altSortingOrders.Add(line[0], int.Parse(line[1]));
        }


        var biomesJson = JsonUtility.FromJson<BiomesJson>(Resources.Load<TextAsset>("BiomeData/biomes").text).biomes;

        tiles = new RuleTile[biomesJson.Length];

        
        biomes = new Biome[biomesJson.Length];
        var biomeAvgRains = new float[biomes.Length];
        var biomeAvgTemps = new float[biomes.Length];
        var biomeRarities = new float[biomes.Length];
        for(int i=0; i<biomes.Length; i++)
        {
            tiles[i] = LoadTile(biomesJson[i].tile_name);
            biomes[i] = new Biome(biomesJson[i]);
            biomeAvgRains[i] = biomesJson[i].avg_rain;
            biomeAvgTemps[i] = biomesJson[i].avg_temp;
            biomeRarities[i] = biomesJson[i].rarity;
        }

        rain_temp_map = new byte[rain_temp_map_width, rain_temp_map_width];
        for(int x=0; x<rain_temp_map_width; x++)
        {
            for(int y=0; y<rain_temp_map_width; y++)
            {
                float closestDist = 1000;
                int closest=0;
                float currentDist;
                for(int i=3; i<biomes.Length; i++)
                {
                    currentDist = Math.Dist(x, y, biomeAvgTemps[i]*rain_temp_map_width, biomeAvgRains[i]*rain_temp_map_width)/biomeRarities[i];
                    if(currentDist < closestDist)
                    {
                        closestDist = currentDist;
                        closest = i;
                    }
                }
                rain_temp_map[x,y] = (byte)closest;
            }
        }

        loadedChunks = new Dictionary<(int, int), GameObject>();
        disabledChunks = new LinkedList<GameObject>();
    }

    static Color32 AverageColorFromTexture(Texture2D tex)
    {
        Color32[] texColors = tex.GetPixels32();

        int total = texColors.Length;
        float r = 0;
        float g = 0;
        float b = 0;

        for(int i = 0; i < total; i++)
        {
            r += texColors[i].r;
            g += texColors[i].g;
            b += texColors[i].b;
        }

        return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 255);
    }

    public static void GenerateTexture(int width)
    {
        var colors = new Color32[tiles.Length];
        for(int i=0; i<tiles.Length; i++)
        {
            colors[i] = AverageColorFromTexture(tiles[i].m_DefaultSprite.texture);
        }
        
        textureBiome = new Texture2D(width, width);
        for(int x=0; x<textureBiome.width; x++)
        {
            for(int y=0; y<textureBiome.height; y++)
            {
                textureBiome.SetPixel(x, y, colors[mapTexture_biome[(int)(((float)x/textureBiome.width)*mapDiameter*chunkSize), (int)(((float)y/textureBiome.height)*mapDiameter*chunkSize)]]);
            }
        }
        textureBiome.Apply();
        
        textureDecor = new Texture2D(width, width);
        for(int x=0; x<textureBiome.width; x++)
        {
            for(int y=0; y<textureBiome.height; y++)
            {
                textureDecor.SetPixel(x, y, mapTexture_decor[(int)(((float)x/textureDecor.width)*mapDiameter*chunkSize), (int)(((float)y/textureDecor.height)*mapDiameter*chunkSize)] == 0 ? Color.white : Color.black);
            }
        }
        textureDecor.Apply();
    }

    const int perlinOffset = 5429; // prevents mirroring

    // returns 0 for water, 1 for shallow water, 2 for sand, 3 for biome gen
    public static byte PerlinMain(Vector2Int pos)
    {
        const float perlinScale = 0.005f;
        float perlinVal = Mathf.PerlinNoise((seed_main + pos.x + perlinOffset)*perlinScale, (seed_main + pos.y + perlinOffset)*perlinScale); // 0 to 1
        perlinVal = Math.Remap(perlinVal, 0, 1, 0.3f, 1);
        float gradientVal = 1-Vector2Int.Distance(pos, center)/(chunkSize*mapRadius); // 1 in center, 0 at edge of map
        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((seed_main + pos.x + perlinOffset)*perlinScaleFine, (seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);
        
        const float landVal = 0.52f;
        const float sandVal = 0.5f;
        const float shallowWaterVal = 0.48f;

        float val = (perlinVal+gradientVal)/2-fineNoise;
        return (byte)(val > landVal ? PerlinBiome(pos) : val > sandVal ? 2 : val > shallowWaterVal ? 1 : 0);
    }

    public static byte PerlinBiome(Vector2Int pos)
    {
        const float perlinScaleRain = 0.003f;
        const float perlinScaleTemp = 0.003f;

        float perlinValRain = Mathf.PerlinNoise((seed_rain + pos.x + perlinOffset)*perlinScaleRain, (seed_rain + pos.y + perlinOffset)*perlinScaleRain);
        float perlinValTemp = Mathf.PerlinNoise((seed_temp + pos.x + perlinOffset)*perlinScaleTemp, (seed_temp + pos.y + perlinOffset)*perlinScaleTemp);

        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((seed_main + pos.x + perlinOffset)*perlinScaleFine, (seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);

        perlinValTemp -= fineNoise;
        perlinValTemp = Mathf.Clamp(Mathf.Round(perlinValTemp * rain_temp_map_width), 0, rain_temp_map_width-1);
        perlinValRain -= fineNoise;
        perlinValRain = Mathf.Clamp(Mathf.Round(perlinValRain * perlinValTemp), 0, perlinValTemp);

        return rain_temp_map[(int)perlinValTemp, (int)perlinValRain];
    }

    static Color32 MultiplyColor(Color32 c, int x)
    {
        return new Color32((byte)(c.r*x), (byte)(c.g*x), (byte)(c.b*x), (byte)(c.a*x));
    }

    public static byte MapClamped(byte[,] map, int x, int y)
    {
        return map[Mathf.Clamp(x, 0, map.GetLength(0)-1), Mathf.Clamp(y, 0, map.GetLength(1)-1)];
    }

    public void GenerateMap()
    {
        Random.InitState(seed_decor);

        var width = mapDiameter*chunkSize;
        mapTexture_biome = new byte[width, width];
        mapTexture_decor = new byte[width, width];

        Vector2Int pos = new Vector2Int(0, 0);
        for(pos.x=0; pos.x<width; pos.x++)
        {
            for(pos.y=0; pos.y<width; pos.y++)
            {
                byte val = PerlinMain(pos);
                mapTexture_biome[pos.x, pos.y] = val;
                mapTexture_decor[pos.x, pos.y] = 0;
                if(val >= 3)
                {
                    float rval = Random.value;
                    for(int i=0; i<biomes[val].decorationThreshholds.Length; i++)
                    {
                        if(rval < biomes[val].decorationThreshholds[i])
                        {
                            mapTexture_decor[pos.x, pos.y] = (byte)(i+1);
                            break;
                        }
                    }
                }
            }
        }
    }

    public static int GetTile(int x, int y)
    {
        return MapClamped(mapTexture_biome, x, y);
    }

    public static GameObject GetDecoration(int x, int y, int tile)
    {
        int i = MapClamped(mapTexture_decor, x, y);
        if(i == 0) return null;
        return biomes[tile].decorations[i-1];
    }

    public static float RandomSeed()
    {
        return Random.Range((float)0, (float)1000000);
    }

    public static void SetSeed(float seed)
    {
        seed = Mathf.Abs(seed);
        seed_main = seed;
        seed_temp = seed_main+530128.3585825032f; // random values have no meaning, just getting a different area in the perlin noise
        seed_rain = seed_main+632571.5362583f; // random values have no meaning, just getting a different area in the perlin noise
        seed_decor = (int)seed_main; // random values have no meaning, just getting a different area in the perlin noise
    }

    void LoadAll()
    {
        for(int x=currPos.x-(renderDistance-1); x<=currPos.x+(renderDistance-1); x++)
        {
            for(int y=currPos.y-(renderDistance-1); y<=currPos.y+(renderDistance-1); y++)
            {
                if(!loadedChunks.ContainsKey((x, y)))
                {
                    if(disabledChunks.Count == 0)
                    {
                        disabledChunks.AddLast(Instantiate(prefab_chunk));
                        disabledChunks.Last.Value.GetComponent<ProceduralChunk>()._Start();
                    }
                    GameObject chunkObj = disabledChunks.First.Value;
                    disabledChunks.RemoveFirst();
                    chunkObj.SetActive(true);
                    chunkObj.transform.localPosition = new Vector3(x*chunkSize, y*chunkSize, 0);
                    var chunk = chunkObj.GetComponent<ProceduralChunk>();
                    chunk.enabled = true;
                    chunk.Init();
                    loadedChunks.Add((x, y), chunkObj);
                }
            }
        }
    }

    void Update()
    {
        currPos.x = (int)Mathf.Floor(PlayerStats.rigidbody.position.x/chunkSize);
        currPos.y = (int)Mathf.Floor(PlayerStats.rigidbody.position.y/chunkSize);
        
        if(currPos != prevPos || loadedChunks.Count == 0 || reset)
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            foreach(var key in new List<(int x, int y)>(loadedChunks.Keys))
            {
                if(Mathf.Abs(key.x-currPos.x) > renderDistance || Mathf.Abs(key.y-currPos.y) > renderDistance || reset)
                {
                    loadedChunks[key].GetComponent<ProceduralChunk>().RemoveDecorations();
                    loadedChunks[key].SetActive(false);
                    disabledChunks.AddLast(loadedChunks[key]);
                    loadedChunks.Remove(key);
                }
            };

            LoadAll();

            reset = false;
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;
    }

    public void CheckLoaded()
    {
        if(!PlayerStats.loadingFirstChunks) return;

        foreach(var pair in loadedChunks)
        {
            if(!pair.Value.GetComponent<ProceduralChunk>().loaded) return;
        }

        FadeTransition.Fade(true, OnFadeComplete);
        PlayerStats.loadingFirstChunks = false;
    }

    public static bool OnFadeComplete()
    {
        PauseHandler.UnPause();
        PauseHandler.UnBlur();
        return true;
    }
}
