using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BiomeJson
{
    public string biome_name;
    public string tile_name;
    public int[] map_color;
    public string[] decorations;
}

[System.Serializable]
public class BiomesJson
{
    public BiomeJson[] biomes;
}

public struct Biome
{
    public TileBase tile;
    public GameObject[] decorations;

    public Biome(BiomeJson json)
    {
        tile = ProceduralGeneration.LoadTile(json.tile_name);
        decorations = new GameObject[json.decorations.Length];
        for(int i=0; i<decorations.Length; i++)
        {
            decorations[i] = Resources.Load<GameObject>("Prefabs/"+json.decorations[i]);
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

    public static Biome[] biomes;
    public static int tex_map_width=100;
    public static byte[,] rain_temp_map;
    public static TileBase tile_sand;
    public static TileBase tile_water;
    public static TileBase tile_water_shallow;

    public static float seed_main; // determines land/water
    public static float seed_temp, seed_rain; // used for biome decision making
    public static float seed_dcor;

    public static TileBase LoadTile(string name)
    {
        return Resources.Load<TileBase>("Tiles/"+name);
    }

    public void Init(float seed)
    {
        instance = this;

        tile_water = LoadTile("water");
        tile_water_shallow = LoadTile("water_shallow");
        tile_sand = LoadTile("sand");

        var rain_temp_map_tex = Resources.Load<Texture2D>("BiomeData/rain_temp_map");

        var biomesJson = JsonUtility.FromJson<BiomesJson>(Resources.Load<TextAsset>("BiomeData/biomes").text).biomes;

        rain_temp_map = new byte[tex_map_width,tex_map_width];
        biomes = new Biome[biomesJson.Length];
        for(int i=0; i<biomes.Length; i++)
        {
            biomes[i] = new Biome(biomesJson[i]);
            for(int x=0; x<tex_map_width; x++)
            {
                for(int y=0; y<tex_map_width; y++)
                {
                    Color c = rain_temp_map_tex.GetPixel(x, y);
                    int[] arr = biomesJson[i].map_color;
                    if(Math.Ish(c.r, arr[0]/255f) && Math.Ish(c.g, arr[1]/255f) && Math.Ish(c.b, arr[2]/255f))
                    {
                        rain_temp_map[x,y] = (byte)i;
                    }
                }
            }
        }

        loadedChunks = new Dictionary<(int, int), GameObject>();
        disabledChunks = new LinkedList<GameObject>();

        SetSeed(seed);
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
        seed_dcor = seed_main+471282.93252735085f; // random values have no meaning, just getting a different area in the perlin noise
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
