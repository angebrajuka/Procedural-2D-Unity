using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicLoading : MonoBehaviour {

    [HideInInspector]
    public Rigidbody2D player_rb;
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos=Vector2Int.zero;
    private Vector2Int bl=Vector2Int.zero, tr=Vector2Int.zero;
    
    void Start() {
        player_rb = GetComponent<Rigidbody2D>();
        
        int posX = (int)(player_rb.position.x/100);
        int posY = (int)(player_rb.position.y/100);

        int alignX = (int)(player_rb.position.x/50)-posX;
        int alignY = (int)(player_rb.position.y/50)-posY;

        if(alignX == 0) {
            tr.x = posX;
            bl.x = posX-1;
        } else {
            bl.x = posX;
            tr.x = posX+1;
        }

        if(alignY == 0) {
            tr.y = posY;
            bl.y = posY-1;
        } else {
            bl.y = posY;
            tr.y = posY+1;
        }
        
        bl = new Vector2Int(2, 0);
        tr = new Vector2Int(3, 1);

        for(int x=bl.x; x<=tr.x; x++)
            for(int y=bl.y; y<=tr.y; y++)
                Load(x, y);
    }

    void Load(int x, int y) {
        try {
            SceneManager.LoadSceneAsync(x+","+y, LoadSceneMode.Additive);
        } catch {}
    }

    void Unload(int x, int y) {
        try {
            SceneManager.UnloadSceneAsync(x+","+y);
        } catch {}
    }

    void Update() {
        
        currPos.x = (int)(player_rb.position.x*3/100);
        currPos.y = (int)(player_rb.position.y*3/100);
        
        if(currPos != prevPos) {

            Application.backgroundLoadingPriority = ThreadPriority.Low;
            
            if((currPos.x+1)/3 > tr.x) {
                // load to right

                for(int y=bl.y; y<=tr.y; y++)
                    Unload(bl.x, y);
                
                bl.x++;
                tr.x++;

                for(int y=bl.y; y<=tr.y; y++)
                    Load(tr.x, y);

            } else if((currPos.x-1)/3 < bl.x) {
                // load to left

                for(int y=bl.y; y<=tr.y; y++)
                    Unload(tr.x, y);
                
                tr.x--;
                bl.x--;
                
                for(int y=bl.y; y<=tr.y; y++)
                    Load(bl.x, y);

            } else if((currPos.y+1)/3 > tr.y) {
                // load up

                for(int x=bl.x; x<=tr.x; x++)
                    Unload(x, bl.y);
                
                bl.y++;
                tr.y++;
                
                for(int x=bl.x; x<=tr.x; x++)
                    Load(x, tr.y);

            } else if((currPos.y-1)/3 < bl.y) {
                // load down

                for(int x=bl.x; x<=tr.x; x++)
                    Unload(x, tr.y);
                
                tr.y--;
                bl.y--;

                for(int x=bl.x; x<=tr.x; x++)
                    Load(x, bl.y);
            }

            // print(currPos);
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;
    }
}
