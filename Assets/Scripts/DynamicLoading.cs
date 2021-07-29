using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicLoading : MonoBehaviour
{
    public GameObject prefab_chunk;

    [HideInInspector]
    public Rigidbody2D player_rb;
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos;
    public const int chunkSize=50;
    public static readonly Vector2Int mapSize = new Vector2Int(20, 30); // chunks
    private Dictionary<(int x, int y), GameObject> loadedChunks;
    private BitArray validChunks;

    bool IsValid(int x, int y) {
        return validChunks.Get(y*mapSize.x+x);
    }

    void Start()
    {
        loadedChunks = new Dictionary<(int, int), GameObject>();
        validChunks = new BitArray(mapSize.x*mapSize.y);
        
        for(int i=1; i<SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            int slash = scenePath.LastIndexOf("/");
            int comma = scenePath.LastIndexOf(",");
            int dot = scenePath.LastIndexOf(".");
            
            if(Int32.TryParse(scenePath.Substring(slash+1, comma-slash-1), out int x) && Int32.TryParse(scenePath.Substring(comma+1, dot-comma-1), out int y))
            {
                validChunks.Set(y*mapSize.x+x, true);
            }
        }

        player_rb = GetComponent<Rigidbody2D>();
    }

    string Name(int x, int y) { return x+","+y; }

    void LoadAll()
    {
        int posX = (int)Mathf.Floor(player_rb.position.x/chunkSize);
        int posY = (int)Mathf.Floor(player_rb.position.y/chunkSize);

        for(int x=Mathf.Max(posX-1, 0); x<=Mathf.Min(posX+1, mapSize.x-1); x++)
        {
            for(int y=Mathf.Max(posY-1, 0); y<=Mathf.Min(posY+1, mapSize.y-1); y++)
            {
                if(!loadedChunks.ContainsKey((x, y))) {
                    
                    loadedChunks.Add((x, y), Instantiate(prefab_chunk, new Vector3(x*chunkSize, y*chunkSize, 0), Quaternion.identity));
                    
                    if(IsValid(x, y))
                    {
                        // replace with fileIO
                        SceneManager.LoadSceneAsync(Name(x, y), LoadSceneMode.Additive);
                    }
                    else
                    {
                        // replace with default chunk
                    }
                }
            }
        }
    }

    void UnloadChunk(int x, int y)
    {
        Destroy(loadedChunks[(x, y)]);
        if(IsValid(x, y))
            SceneManager.UnloadSceneAsync(Name(x, y));
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

            LinkedList<(int x, int y)> toUnload = new LinkedList<(int x, int y)>();
            foreach(var chunk in loadedChunks)
            {
                if(Mathf.Abs(chunk.Key.x-currPos.x) > 2 || Mathf.Abs(chunk.Key.y-currPos.y) > 2)
                {
                    toUnload.AddLast(chunk.Key);
                }
            };

            foreach((int x, int y) key in toUnload)
            {
                UnloadChunk(key.x, key.y);
                loadedChunks.Remove(key);
            }

            

            LoadAll();
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;
    }
}
