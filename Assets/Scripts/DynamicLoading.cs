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
    // private Vector2Int bl=Vector2Int.zero, tr=Vector2Int.zero;
    private HashSet<(int, int)> loaded;
    private HashSet<(int, int)> validScenes;

    void Start()
    {
        loaded = new HashSet<(int, int)>();
        validScenes = new HashSet<(int, int)>();
        
        for(int i=2; i<SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            int slash = scenePath.LastIndexOf("/");
            int comma = scenePath.LastIndexOf(",");
            int dot = scenePath.LastIndexOf(".");
            
            if(Int32.TryParse(scenePath.Substring(slash+1, comma-slash-1), out int x) && Int32.TryParse(scenePath.Substring(comma+1, dot-comma-1), out int y))
            {
                validScenes.Add((x, y));
            }
        }

        player_rb = GetComponent<Rigidbody2D>();
    }

    string Name(int x, int y) { return x+","+y; }

    void LoadAll()
    {
        int posX = (int)Mathf.Floor(player_rb.position.x/100);
        int posY = (int)Mathf.Floor(player_rb.position.y/100);

        
        for(int x=posX-1; x<=posX+1; x++)
        {
            for(int y=posY-1; y<=posY+1; y++)
            {
                if(!loaded.Contains((x, y)) && validScenes.Contains((x, y))) {
                    SceneManager.LoadSceneAsync(Name(x, y), LoadSceneMode.Additive);
                    loaded.Add((x, y));
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

            loaded.RemoveWhere(delegate((int x, int y) tuple)
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
