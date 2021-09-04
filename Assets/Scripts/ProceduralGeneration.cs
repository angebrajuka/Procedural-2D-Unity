using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Biome
{
    
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
    public const int mapRadius=10;
    public const int mapDiameter=mapRadius*2;
    public static readonly Vector2Int center = Vector2Int.one*mapRadius*chunkSize;
    public static Dictionary<(int x, int y), GameObject> loadedChunks;
    public static LinkedList<GameObject> disabledChunks;
    public int renderDistance;
    public static bool reset=true;

    public static TileBase[] tiles_land;
    public static TileBase tile_sand;
    public static TileBase tile_water;
    public static TileBase tile_water_shallow;

    public static float seed_main; // determines land/water
    public static float seed_temperature, seed_rainfall; // used for biome decision making
    public static float seed_decorations;

    public void Init(float seed)
    {
        instance = this;

        UnityEngine.Object[] objTiles = Resources.LoadAll("Tiles/Ground/Land", typeof(TileBase));
        tiles_land = new TileBase[objTiles.Length];
        for(int i=0; i<objTiles.Length; i++)
        {
            tiles_land[i] = (TileBase)objTiles[i];
        }
        tile_water = (TileBase)Resources.Load("Tiles/Ground/Water/water", typeof(TileBase));
        tile_water_shallow = (TileBase)Resources.Load("Tiles/Ground/Water/water_shallow", typeof(TileBase));
        tile_sand = (TileBase)Resources.Load("Tiles/Ground/Land/sand", typeof(TileBase));


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
        seed_temperature = seed_main+530128.3585825032f; // random values have no meaning, just getting a different area in the perlin noise
        seed_rainfall = seed_main+632571.5362583f; // random values have no meaning, just getting a different area in the perlin noise
        seed_decorations = seed_main+471282.93252735085f; // random values have no meaning, just getting a different area in the perlin noise
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
