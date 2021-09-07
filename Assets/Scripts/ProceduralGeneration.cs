using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BiomeJson
{
    public string biome_name;
    public string tile_name;
    public int[] rain_temp_map_color;
    public string[] decorations;
    public float[] decorationChances;
}

[System.Serializable]
public class BiomesJson
{
    public BiomeJson[] biomes;
}

public struct Biome
{
    public GameObject[] decorations;
    public float[] decorationThreshholds;

    public Biome(BiomeJson json)
    {
        decorations = new GameObject[json.decorations.Length];
        for(int i=0; i<decorations.Length; i++)
        {
            decorations[i] = Resources.Load<GameObject>("Decorations/"+json.decorations[i]);
        }
        decorationThreshholds = new float[json.decorationChances.Length];
        for(int i=0; i<decorationThreshholds.Length; i++)
        {
            decorationThreshholds[i] = json.decorationChances[i];
            decorationThreshholds[i] += i > 0 ? decorationThreshholds[i-1] : 0;
        }
    }
}

public class ProceduralGeneration : MonoBehaviour
{
    static readonly Color32 WHITE = new Color32(1, 1, 1, 255);
    static readonly Color32 RED = new Color32(1, 0, 0, 255);
    static readonly Color32 GREEN = new Color32(0, 1, 0, 255);
    static readonly Color32 BLUE = new Color32(0, 0, 1, 255);
    static readonly Color32 BLACK = new Color32(0, 0, 0, 255);

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
    public static int tex_map_width=100;
    public static byte[,] rain_temp_map;

    // public static Color32[] colors_land;
    // public static Color32[] colors_beach;
    public Texture2D mapTexture_biome;
    public Texture2D mapTexture_decor;

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
        var biomesJson = JsonUtility.FromJson<BiomesJson>(Resources.Load<TextAsset>("BiomeData/biomes").text).biomes;

        tiles = new RuleTile[3+biomesJson.Length];
        tiles[0] = LoadTile("water");
        tiles[1] = LoadTile("water_shallow");
        tiles[2] = LoadTile("sand");

        rain_temp_map = new byte[tex_map_width,tex_map_width];
        biomes = new Biome[biomesJson.Length];
        for(int i=0; i<biomes.Length; i++)
        {
            tiles[i+3] = ProceduralGeneration.LoadTile(biomesJson[i].tile_name);
            biomes[i] = new Biome(biomesJson[i]);
            for(int x=0; x<tex_map_width; x++)
            {
                for(int y=0; y<tex_map_width; y++)
                {
                    Color32 c = rain_temp_map_tex.GetPixel(x, y);
                    int[] arr = biomesJson[i].rain_temp_map_color;
                    if(c.r == arr[0] && c.g == arr[1] && c.b == arr[2])
                    {
                        rain_temp_map[x,y] = (byte)i;
                    }
                }
            }
        }

        loadedChunks = new Dictionary<(int, int), GameObject>();
        disabledChunks = new LinkedList<GameObject>();

        
    }

    const int perlinOffset = 5429; // prevents mirroring

    // returns 0 for water, 1 for shallow water, 2 for sand, 3 for biome gen
    public static int PerlinMain(Vector2Int pos)
    {
        const float perlinScale = 0.01f;
        float perlinVal = Mathf.PerlinNoise((ProceduralGeneration.seed_main + pos.x + perlinOffset)*perlinScale, (ProceduralGeneration.seed_main + pos.y + perlinOffset)*perlinScale); // 0 to 1
        perlinVal = Math.Remap(perlinVal, 0, 1, 0.3f, 1);
        float gradientVal = 1-Vector2Int.Distance(pos, ProceduralGeneration.center)/(ProceduralGeneration.chunkSize*ProceduralGeneration.mapRadius); // 1 in center, 0 at edge of map
        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((ProceduralGeneration.seed_main + pos.x + perlinOffset)*perlinScaleFine, (ProceduralGeneration.seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);
        
        const float landVal = 0.52f;
        const float sandVal = 0.5f;
        const float shallowWaterVal = 0.48f;

        float val = (perlinVal+gradientVal)/2-fineNoise;
        return val > landVal ? 3 : val > sandVal ? 2 : val > shallowWaterVal ? 1 : 0;
    }

    public static int PerlinBiome(Vector2Int pos)
    {
        const float perlinScaleRain = 0.003f;
        const float perlinScaleTemp = 0.003f;

        float perlinValRain = Mathf.PerlinNoise((ProceduralGeneration.seed_rain + pos.x + perlinOffset)*perlinScaleRain, (ProceduralGeneration.seed_rain + pos.y + perlinOffset)*perlinScaleRain);
        float perlinValTemp = Mathf.PerlinNoise((ProceduralGeneration.seed_temp + pos.x + perlinOffset)*perlinScaleTemp, (ProceduralGeneration.seed_temp + pos.y + perlinOffset)*perlinScaleTemp);

        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((ProceduralGeneration.seed_main + pos.x + perlinOffset)*perlinScaleFine, (ProceduralGeneration.seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);

        perlinValTemp -= fineNoise;
        perlinValTemp = Mathf.Clamp(Mathf.Round(perlinValTemp * ProceduralGeneration.tex_map_width), 0, ProceduralGeneration.tex_map_width-1);
        perlinValRain -= fineNoise;
        perlinValRain = Mathf.Clamp(Mathf.Round(perlinValRain * ProceduralGeneration.tex_map_width), 0, perlinValTemp);

        return ProceduralGeneration.rain_temp_map[(int)perlinValTemp, (int)perlinValRain];
    }

    static Color32 MultiplyColor(Color32 c, int x)
    {
        return new Color32((byte)(c.r*x), (byte)(c.g*x), (byte)(c.b*x), (byte)(c.a*x));
    }

    public void GenerateMap()
    {
        Random.InitState(seed_decor);

        var width = mapDiameter*chunkSize;
        mapTexture_biome = new Texture2D(width, width);
        mapTexture_decor = new Texture2D(width, width);
        mapTexture_biome.wrapMode = TextureWrapMode.Clamp;
        mapTexture_decor.wrapMode = TextureWrapMode.Clamp;

        Vector2Int pos = new Vector2Int(0, 0);
        for(pos.x=0; pos.x<width; pos.x++)
        {
            for(pos.y=0; pos.y<width; pos.y++)
            {
                int val = PerlinMain(pos);
                int biome = PerlinBiome(pos);
                mapTexture_decor.SetPixel(pos.x, pos.y, MultiplyColor(WHITE, 0));
                if(val == 3)
                {
                    mapTexture_biome.SetPixel(pos.x, pos.y, MultiplyColor(WHITE, biome+val));

                    float rval = Random.value;
                    for(int i=0; i<biomes[biome].decorationThreshholds.Length; i++)
                    {
                        if(rval < biomes[biome].decorationThreshholds[i])
                        {
                            mapTexture_decor.SetPixel(pos.x, pos.y, MultiplyColor(WHITE, i+1));
                        }
                    }
                }
                else
                {
                    mapTexture_biome.SetPixel(pos.x, pos.y, MultiplyColor(WHITE, val));
                }
            }
        }
        mapTexture_biome.Apply();
        mapTexture_decor.Apply();
    }

    public static int GetTile(int x, int y)
    {
        Color32 c = instance.mapTexture_biome.GetPixel(x, y);
        return c.r;
    }

    public static GameObject GetDecoration(int x, int y, int tile)
    {
        Color32 c = instance.mapTexture_decor.GetPixel(x, y);
        if(c.r == 0) return null;
        return biomes[tile-3].decorations[c.r-1];
    }

    public static float RandomSeed()
    {
        return Random.Range((float)(-1000000), (float)1000000);
    }

    public static void SetSeed(float seed)
    {
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
