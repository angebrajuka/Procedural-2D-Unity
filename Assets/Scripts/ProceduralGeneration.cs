using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    // hierarchy
    // public GameObject prefab_chunk_loading;
    // public GameObject prefab_chunk_unloading;
    public Tilemap tilemap_ground;

    public static ProceduralGeneration instance;
    
    [HideInInspector]
    public Rigidbody2D player_rb;
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos;
    public const int chunkSize=50;
    public const int mapRadius=16;
    public const int mapDiameter=mapRadius*2;
    public static readonly Vector2Int center = Vector2Int.one*ProceduralGeneration.mapRadius;
    public static Dictionary<(int x, int y), ProceduralChunkLoader> loadingChunks;
    public static Dictionary<(int x, int y), ProceduralChunkUnloader> unloadingChunks;
    public static HashSet<(int x, int y)> loadedChunks;
    public static TileBase[] tiles;
    public static int seed;

    public void Init()
    {
        instance = this;

        UnityEngine.Object[] objTiles = Resources.LoadAll("Tiles/Ground", typeof(TileBase));
        tiles = new TileBase[objTiles.Length];
        for(int i=0; i<objTiles.Length; i++)
        {
            tiles[i] = (TileBase)objTiles[i];
        }

        loadingChunks = new Dictionary<(int, int), ProceduralChunkLoader>();
        unloadingChunks = new Dictionary<(int x, int y), ProceduralChunkUnloader>();
        loadedChunks = new HashSet<(int x, int y)>();

        player_rb = PlayerStats.rigidbody;
    }

    void LoadAll()
    {
        int posX = (int)Mathf.Floor(player_rb.position.x/chunkSize);
        int posY = (int)Mathf.Floor(player_rb.position.y/chunkSize);

        for(int x=Mathf.Max(posX-1, 0); x<=Mathf.Min(posX+1, mapDiameter-1); x++)
        {
            for(int y=Mathf.Max(posY-1, 0); y<=Mathf.Min(posY+1, mapDiameter-1); y++)
            {
                if(!loadedChunks.Contains((x, y)) && !loadingChunks.ContainsKey((x, y)))
                {
                    GameObject chunkObj = new GameObject();
                    chunkObj.AddComponent<ProceduralChunkLoader>();
                    var chunk = chunkObj.GetComponent<ProceduralChunkLoader>();
                    chunk.pos = new Vector3Int(x, y, 0);
                    loadingChunks.Add((x, y), chunk);
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



            foreach(var key in loadedChunks)
            {
                if(Mathf.Abs(key.x-currPos.x) > 2 || Mathf.Abs(key.y-currPos.y) > 2)
                {
                    GameObject chunkObj = new GameObject();
                    chunkObj.AddComponent<ProceduralChunkUnloader>();
                    var chunk = chunkObj.GetComponent<ProceduralChunkUnloader>();
                    chunk.pos = new Vector3Int(key.x, key.y, 0);
                    unloadingChunks.Add((key.x, key.y), chunk);
                }
            };

            LoadAll();
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;
    }

    public void CheckLoaded(ProceduralChunkLoader loader)
    {
        loadedChunks.Add((loader.pos.x, loader.pos.y));
        loadingChunks.Remove((loader.pos.x, loader.pos.y));
        
        if(loadingChunks.Count != 0) return;

        tilemap_ground.CompressBounds();

        if(PlayerStats.loadingFirstChunks)
        {
            FadeTransition.Fade(true, OnFadeComplete);
            PlayerStats.loadingFirstChunks = false;
        }
    }

    public static bool OnFadeComplete()
    {
        PauseHandler.UnPause();
        PauseHandler.UnBlur();
        return true;
    }
}
