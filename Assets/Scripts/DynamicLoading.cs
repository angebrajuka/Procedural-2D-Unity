using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.IO;

public class DynamicLoading : MonoBehaviour
{
    public GameObject prefab_chunk;
    public TileBase defaultTile;

    [HideInInspector]
    public Rigidbody2D player_rb;
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos;
    public const int chunkSize=50;
    public static readonly Vector2Int mapSize = new Vector2Int(20, 30); // chunks
    private Dictionary<(int x, int y), GameObject> loadedChunks;
    private static BitArray validChunks;
    public static TileBase[] tiles;

    public static bool IsValid(int x, int y) {
        return validChunks.Get(y*mapSize.x+x);
    }

    public void Init()
    {
        UnityEngine.Object[] objTiles = Resources.LoadAll("Tiles/Ground", typeof(TileBase));
        tiles = new TileBase[objTiles.Length];
        for(int i=0; i<objTiles.Length; i++)
        {
            tiles[i] = (TileBase)objTiles[i];
        }

        loadedChunks = new Dictionary<(int, int), GameObject>();
        validChunks = new BitArray(mapSize.x*mapSize.y);

        TextAsset validChunksTxt = (TextAsset)Resources.Load("ValidChunks");
        string[] validChunksTxtArr = validChunksTxt.text.Split(';');

        foreach(var chunk in validChunksTxtArr)
        {
            string[] xy = chunk.Split(',');
            if(Int32.TryParse(xy[0], out int x) && Int32.TryParse(xy[1], out int y))
            {
                validChunks.Set(y*mapSize.x+x, true);
            }
        }

        player_rb = GetComponent<Rigidbody2D>();
    }

    public static string Name(int x, int y) { return x+","+y; }

    public GameObject InstantiateChunk(int x, int y)
    {
        GameObject gameObject = Instantiate(prefab_chunk, new Vector3(x*chunkSize, y*chunkSize, 0), Quaternion.identity);
        Chunk chunk = gameObject.GetComponent<Chunk>();

        chunk.x = x;
        chunk.y = y;
        chunk.dynamicLoader = this;

        return gameObject;
    }

    void LoadAll()
    {
        int posX = (int)Mathf.Floor(player_rb.position.x/chunkSize);
        int posY = (int)Mathf.Floor(player_rb.position.y/chunkSize);

        for(int x=Mathf.Max(posX-1, 0); x<=Mathf.Min(posX+1, mapSize.x-1); x++)
        {
            for(int y=Mathf.Max(posY-1, 0); y<=Mathf.Min(posY+1, mapSize.y-1); y++)
            {
                if(!loadedChunks.ContainsKey((x, y)))
                {
                    GameObject chunk = InstantiateChunk(x, y);
                    loadedChunks.Add((x, y), chunk);
                }
            }
        }
    }

    void UnloadChunk(int x, int y)
    {
        Destroy(loadedChunks[(x, y)]);
    }

    void OnDisable()
    {
        foreach(KeyValuePair<(int x, int y), GameObject> chunk in loadedChunks)
        {
            UnloadChunk(chunk.Key.x, chunk.Key.y);
        }
        loadedChunks.Clear();
    }

    void OnEnable()
    {
        prevPos=Vector2Int.one*-100;
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
                    UnloadChunk(key.x, key.y);
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
            if(!pair.Value.GetComponent<Chunk>().loaded) return;
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
