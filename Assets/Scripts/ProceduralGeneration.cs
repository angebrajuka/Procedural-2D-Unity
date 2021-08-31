using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_chunk;
    public TileBase water;
    public TileBase grass;

    public static ProceduralGeneration instance;
    
    [HideInInspector]
    public Rigidbody2D player_rb;
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos;
    public const int chunkSize=50;
    public const int mapRadius=5;
    public const int mapDiameter=mapRadius*2;
    public static readonly Vector2Int center = Vector2Int.one*mapRadius*chunkSize;
    public static Dictionary<(int x, int y), GameObject> loadedChunks;
    public static LinkedList<GameObject> disabledChunks;
    public int renderDistance;

    public static TileBase[] tiles;

    public static float seed_main; // determines land/water
    public static float seed_biome;
    public static float seed_decorations;

    public void Init(float seed)
    {
        instance = this;

        UnityEngine.Object[] objTiles = Resources.LoadAll("Tiles/Ground", typeof(TileBase));
        tiles = new TileBase[objTiles.Length];
        for(int i=0; i<objTiles.Length; i++)
        {
            tiles[i] = (TileBase)objTiles[i];
        }

        loadedChunks = new Dictionary<(int, int), GameObject>();
        disabledChunks = new LinkedList<GameObject>();

        player_rb = PlayerStats.rigidbody;

        SetSeed(seed);
    }

    public static float RandomSeed()
    {
        return Random.Range((float)(-1000000), (float)1000000);
    }

    public static void SetSeed(float seed)
    {
        seed_main = seed;
        seed_biome = seed_main+0.3585825032f; // random values have no meaning, just not the same as main
        seed_decorations = seed_main+0.93252735085f; // random values have no meaning, just not the same as main
    }

    void LoadAll()
    {
        // for(int x=Mathf.Max(currPos.x-(renderDistance-1), 0); x<=Mathf.Min(currPos.x+(renderDistance-1), mapDiameter-1); x++)
        // {
        //     for(int y=Mathf.Max(currPos.y-(renderDistance-1), 0); y<=Mathf.Min(currPos.y+(renderDistance-1), mapDiameter-1); y++)
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
        currPos.x = (int)Mathf.Floor(player_rb.position.x/chunkSize);
        currPos.y = (int)Mathf.Floor(player_rb.position.y/chunkSize);
        
        if(currPos != prevPos || loadedChunks.Count == 0)
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            // tilemap_ground.origin = Math.Vec3(currPos-(Vector2Int.one*2))*chunkSize;
            // tilemap_ground.size = Math.Vec3(Vector2Int.one*5*chunkSize)+Vector3Int.forward;
            // tilemap_ground.ResizeBounds();

            foreach(var key in new List<(int x, int y)>(loadedChunks.Keys))
            {
                if(Mathf.Abs(key.x-currPos.x) > renderDistance || Mathf.Abs(key.y-currPos.y) > renderDistance)
                {
                    loadedChunks[key].SetActive(false);
                    disabledChunks.AddLast(loadedChunks[key]);
                    loadedChunks.Remove(key);
                }
            };

            LoadAll();
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
