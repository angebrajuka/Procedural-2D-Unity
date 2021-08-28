using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    // hierarchy
    public GameObject prefab_chunk;
    public Tilemap tilemap_ground;

    public static ProceduralGeneration instance;
    
    [HideInInspector]
    public Rigidbody2D player_rb;
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos;
    public const int chunkSize=50;
    public const int mapRadius=16;
    public const int mapDiameter=mapRadius*2;
    private Dictionary<(int x, int y), ProceduralChunk> loadedChunks;
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

        loadedChunks = new Dictionary<(int, int), ProceduralChunk>();

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
                if(!loadedChunks.ContainsKey((x, y)))
                {
                    GameObject chunkObj = Instantiate(prefab_chunk);
                    ProceduralChunk chunk = chunkObj.GetComponent<ProceduralChunk>();
                    chunk.pos = new Vector2Int(x, y);
                    loadedChunks.Add((x, y), chunk);
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

            foreach(var key in new List<(int x, int y)>(loadedChunks.Keys))
            {
                if(Mathf.Abs(key.x-currPos.x) > 2 || Mathf.Abs(key.y-currPos.y) > 2)
                {
                    loadedChunks[key].state = ProceduralChunk.State.UNLOADING;
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
        foreach(var pair in loadedChunks)
        {
            if(pair.Value.state != ProceduralChunk.State.LOADED) return;
        }

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
