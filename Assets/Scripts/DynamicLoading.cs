using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicLoading : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D player_rb;
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos=Vector2Int.one*-100;
    private const int mapWidth=100;
    private const int mapHeight=150;
    private HashSet<(int, int)> loadedScenes;
    private BitArray validScenes;

    bool IsValid(int x, int y) {
        return validScenes.Get(y*mapWidth+x);
    }

    void Start()
    {
        loadedScenes = new HashSet<(int, int)>();
        validScenes = new BitArray(mapWidth*mapHeight);
        
        for(int i=2; i<SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            int slash = scenePath.LastIndexOf("/");
            int comma = scenePath.LastIndexOf(",");
            int dot = scenePath.LastIndexOf(".");
            
            if(Int32.TryParse(scenePath.Substring(slash+1, comma-slash-1), out int x) && Int32.TryParse(scenePath.Substring(comma+1, dot-comma-1), out int y))
            {
                validScenes.Set(y*mapWidth+x, true);
            }
        }

        player_rb = GetComponent<Rigidbody2D>();
    }

    string Name(int x, int y) { return x+","+y; }

    void LoadAll()
    {
        int posX = (int)Mathf.Floor(player_rb.position.x/100);
        int posY = (int)Mathf.Floor(player_rb.position.y/100);

        for(int x=Mathf.Max(posX-1, 0); x<=Mathf.Min(posX+1, mapWidth-1); x++)
        {
            for(int y=Mathf.Max(posY-1, 0); y<=Mathf.Min(posY+1, mapHeight-1); y++)
            {
                if(!loadedScenes.Contains((x, y)) && IsValid(x, y)) {
                    SceneManager.LoadSceneAsync(Name(x, y), LoadSceneMode.Additive);
                    loadedScenes.Add((x, y));
                }
            }
        }
    }

    void Update()
    {    
        currPos.x = (int)(player_rb.position.x/100);
        currPos.y = (int)(player_rb.position.y/100);    // each scene is split into 9 chunks, prevents crossing back and forth across a line to load and unload too fast
        
        if(currPos != prevPos)
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            loadedScenes.RemoveWhere(delegate((int x, int y) tuple)
            {
                if(Mathf.Abs(tuple.x-currPos.x) > 2 || Mathf.Abs(tuple.y-currPos.y) > 2) {
                    try { SceneManager.UnloadSceneAsync(Name(tuple.x, tuple.y)); } catch {}
                    return true;
                }
                return false;
            });

            LoadAll();
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;
    }
}
