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
    public const int mapRadius=4;
    public const int mapDiameter=mapRadius*2;
    public static readonly Vector3Int center = new Vector3Int(mapRadius, mapRadius, 0);
    public static Dictionary<(int x, int y), GameObject> loadedChunks;
    public static LinkedList<GameObject> disabledChunks;
    public static TileBase[] tiles;

    public static int seed_main; // determines land/water
    public static int seed_biome;
    public static int seed_decorations;

    public void Init(int seed)
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
        for(int i=0; i<16; i++)
        {
            disabledChunks.AddLast(Instantiate(prefab_chunk));
            disabledChunks.Last.Value.GetComponent<ProceduralChunk>()._Start();
            disabledChunks.Last.Value.SetActive(false);
        }

        player_rb = PlayerStats.rigidbody;

        seed_main = seed;
        seed_biome = seed_main+109358; // random values have no meaning, just not the same as main
        seed_decorations = seed_main+349085; // random values have no meaning, just not the same as main
    }

    void LoadAll()
    {
        int posX = (int)Mathf.Floor(player_rb.position.x/chunkSize);
        int posY = (int)Mathf.Floor(player_rb.position.y/chunkSize);

        for(int x=Mathf.Max(posX-1, 0); x<=Mathf.Min(posX+1, mapDiameter-1); x++)
        {
            for(int y=Mathf.Max(posY-1, 0); y<=Mathf.Min(posY+1, mapDiameter-1); y++)
            {
                if(!loadedChunks.ContainsKey((x, y)))
                {
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
        currPos.x = (int)(player_rb.position.x/chunkSize);
        currPos.y = (int)(player_rb.position.y/chunkSize);
        
        if(currPos != prevPos)
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            // tilemap_ground.origin = Math.Vec3(currPos-(Vector2Int.one*2))*chunkSize;
            // tilemap_ground.size = Math.Vec3(Vector2Int.one*5*chunkSize)+Vector3Int.forward;
            // tilemap_ground.ResizeBounds();

            foreach(var key in new List<(int x, int y)>(loadedChunks.Keys))
            {
                if(Mathf.Abs(key.x-currPos.x) > 2 || Mathf.Abs(key.y-currPos.y) > 2)
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
