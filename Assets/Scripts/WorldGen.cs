using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

using static Singles;

[System.Serializable]
public struct MenuSeedPosition {
    public ushort seed;
    public float x, y;
}

public class WorldGen : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_chunk;
    public GameObject prefab_mask;
    public Sprite[] sprite_masks;
    public GameObject prefab_decoration;
    public Transform chunks;
    public Transform decorPrefabs;
    public MenuSeedPosition[] menuSeeds;

    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos;
    public const int chunkSize=50;
    public const int mapRadius=16;
    public const int mapDiameter=mapRadius*2;
    public static readonly Vector2Int center = Vector2Int.one*mapRadius*chunkSize;
    public static Vector2 playerSpawnPoint = Vector2.zero;
    public static Dictionary<(int x, int y), Chunk> loadedChunks;
    public static LinkedList<Chunk> disabledChunks;
    public int renderDistance;
    public static bool reset=true;
    public static bool loadingFirstChunks;

    public static RuleTile[] tiles;
    public static HashSet<int> s_shallowWater;
    public static Biome[] biomes;
    public static int rain_temp_map_width=100;
    public static byte[,] rain_temp_map;

    public static byte[,] mapTexture_biome;
    public static byte[,] mapTexture_decor;

    public static Texture2D textureBiome;
    public static Texture2D textureDecor;

    public static ushort seed;
    public static float seed_main, seed_temp, seed_rain;
    public static int seed_decor;

    public static RuleTile LoadTile(string name)
    {
        return Resources.Load<RuleTile>("Tiles/"+name);
    }

    public static void Clear()
    {
        textureBiome = null;
        textureDecor = null;
        foreach(var pair in Biome.s_decorations)
        {
            Destroy(pair.Value.gameObject);
        }
    }

    public void Start()
    {
        Biome.Init();


        var rain_temp_map_tex = Resources.Load<Texture2D>("BiomeData/rain_temp_map");
        var biomesJson = JsonUtility.FromJson<BiomesJson>(Resources.Load<TextAsset>("BiomeData/biomes").text).biomes;

        tiles = new RuleTile[biomesJson.Length];
        biomes = new Biome[biomesJson.Length];
        s_shallowWater = new HashSet<int>();
        rain_temp_map = new byte[rain_temp_map_width, rain_temp_map_width];

        for(int i=0; i<biomes.Length; i++)
        {
            tiles[i] = LoadTile(biomesJson[i].tile_name);
            if(biomesJson[i].tile_name.Equals("water_shallow"))
            {
                s_shallowWater.Add(i);
            }
            biomes[i] = new Biome(biomesJson[i]);
            if(biomesJson[i].rain_temp_map_color.Length == 3)
            {
                for(int x=0; x<rain_temp_map_width; x++)
                {
                    for(int y=0; y<rain_temp_map_width; y++)
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
        }

        loadedChunks = new Dictionary<(int, int), Chunk>();
        disabledChunks = new LinkedList<Chunk>();

        while(disabledChunks.Count < Math.Sqr(renderDistance*2))
        {
            disabledChunks.AddLast(Instantiate(prefab_chunk, chunks).GetComponent<Chunk>());
            disabledChunks.Last.Value._Start();
            disabledChunks.Last.Value.gameObject.SetActive(false);
        }

        var width = mapDiameter*chunkSize;
        mapTexture_biome = new byte[width, width];
        mapTexture_decor = new byte[width, width];
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
        const float perlinScale = 0.004f;
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
        perlinValTemp = Mathf.Round(perlinValTemp * rain_temp_map_width);
        perlinValRain -= fineNoise;
        perlinValRain = Mathf.Round(perlinValRain * rain_temp_map_width);

        return MapClamped(rain_temp_map, (int)perlinValTemp, (int)perlinValRain);
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
        var state = Random.state;

        Random.InitState(seed_decor);

        Vector2Int pos = new Vector2Int(0, 0);
        var width = mapDiameter*chunkSize;
        for(pos.x=0; pos.x<width; pos.x++)
        {
            for(pos.y=0; pos.y<width; pos.y++)
            {
                byte val = PerlinMain(pos);
                mapTexture_biome[pos.x, pos.y] = val;
                mapTexture_decor[pos.x, pos.y] = 254;
            }
        }

        var psp = Random.insideUnitCircle.normalized;
        playerSpawnPoint = center + psp*mapRadius*chunkSize;
        int jic;
        for(jic=0; jic < 999999 && GetTile((int)playerSpawnPoint.x, (int)playerSpawnPoint.y) <= 2; jic++) { // biome 0,1,2 is ocean,shoreline,beach
            playerSpawnPoint -= psp*6;
        }
        for(jic=0; jic<1000 && GetTile((int)playerSpawnPoint.x, (int)playerSpawnPoint.y) != 1; jic++) {
            playerSpawnPoint += psp;
        }
        

        playerSpawnPoint += Random.insideUnitCircle*3;

        for(pos.x=0; pos.x<width; pos.x++)
        {
            for(pos.y=0; pos.y<width; pos.y++)
            {
                int val = mapTexture_biome[pos.x, pos.y];

                if(mapTexture_decor[pos.x, pos.y] == 254)
                {
                    float rval = Random.value;
                    for(int i=0; i<biomes[val].decorationThreshholds.Length; i++)
                    {
                        if(rval < biomes[val].decorationThreshholds[i])
                        {
                            for(int x=0; x<biomes[val].decorations[i].stats.renderSize.x; x++)
                            {
                                for(int y=0; y<biomes[val].decorations[i].stats.renderSize.y; y++)
                                {
                                    if(MapClamped(mapTexture_biome, pos.x+x, pos.y+y) != val ||
                                    (MapClamped(mapTexture_decor, pos.x+x, pos.y+y) != 254 &&
                                    (MapClamped(mapTexture_decor, pos.x+x, pos.y+y) == 255 ||
                                    biomes[val].decorations[MapClamped(mapTexture_decor, pos.x+x, pos.y+y)].stats.collider != null)))
                                    {
                                        goto BreakBreak;
                                    }
                                }
                            }
                            for(int x=0; x<biomes[val].decorations[i].stats.size.x; x++)
                            {
                                for(int y=0; y<biomes[val].decorations[i].stats.size.y; y++)
                                {
                                    mapTexture_decor[pos.x+x, pos.y+y] = 255;
                                }
                            }
                            mapTexture_decor[pos.x, pos.y] = (byte)(i);
                            BreakBreak:
                            break;
                        }
                    }
                }
            }
        }
        loadingFirstChunks = true;
        reset = true;

        Random.state = state;
    }

    public static int GetTile(int x, int y)
    {
        return MapClamped(mapTexture_biome, x, y);
    }

    public static void SpawnDecoration(int x, int y, int tile, Transform parent)
    {
        int i = MapClamped(mapTexture_decor, x, y);
        if(i == 254 || i == 255) return;

        var decoration = biomes[tile].decorations[i];
        var go = Instantiate(decoration.gameObject, new Vector3(x, y, 0), Quaternion.identity, parent);
        go.GetComponent<Decoration>().stats = decoration.stats; // not cloned because reference?
        go.SetActive(true);
    }

    public static ushort RandomSeed()
    {
        return (ushort)Random.Range(0, 1000000);
    }

    public static void SetSeed(ushort seed)
    {
        WorldGen.seed = seed;

        seed_main = 2589.216f+seed*252.3457f;
        seed_rain = 913.8473f+seed*2345.195f;
        seed_temp = 111.8325f+seed*762.0934f;
        seed_decor = seed;
    }

    void LoadAll()
    {
        for(int x=currPos.x-(renderDistance-1); x<=currPos.x+(renderDistance-1); x++)
        {
            for(int y=currPos.y-(renderDistance-1); y<=currPos.y+(renderDistance-1); y++)
            {
                if(!loadedChunks.ContainsKey((x, y)))
                {
                    var chunk = disabledChunks.First.Value;
                    var chunkObj = disabledChunks.First.Value.gameObject;
                    disabledChunks.RemoveFirst();
                    chunkObj.SetActive(true);
                    chunkObj.transform.localPosition = new Vector3(x*chunkSize, y*chunkSize, 0);
                    chunk.enabled = true;
                    chunk.Init();
                    loadedChunks.Add((x, y), chunk);
                }
            }
        }
    }

    void Update()
    {
        currPos.x = (int)Mathf.Floor(singles.cameraFollow.transform.position.x/chunkSize);
        currPos.y = (int)Mathf.Floor(singles.cameraFollow.transform.position.y/chunkSize);
        
        if(currPos != prevPos || loadedChunks.Count == 0 || reset)
        {
            foreach(var key in new List<(int x, int y)>(loadedChunks.Keys))
            {
                if(Mathf.Abs(key.x-currPos.x) > renderDistance || Mathf.Abs(key.y-currPos.y) > renderDistance || reset)
                {
                    loadedChunks[key].RemoveDecorations();
                    loadedChunks[key].gameObject.SetActive(false);
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

    public async void CheckLoaded()
    {
        if(!loadingFirstChunks) return;

        foreach(var pair in loadedChunks)
        {
            if(!pair.Value.loaded) return;
        }

        FadeTransition.black = false;
        PauseHandler.UnPause();
        PauseHandler.blurred = false;
        loadingFirstChunks = false;

        if(singles.menuCampfire.gameObject.activeSelf) {
            await Task.Delay(200);
            singles.menuCampfire.Lit = true;
        }
    }
}
