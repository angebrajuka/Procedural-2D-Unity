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



        bl = new Vector2Int(2, 0);
        tr = new Vector2Int(3, 1);
    }

    void Update() {
        
        currPos.x = (int)(player_rb.position.x*3/100);
        currPos.y = (int)(player_rb.position.y*3/100);
        
        if(currPos != prevPos) {
            
            try {
                if((currPos.x+1)/3 > tr.x) {
                    // load to right

                    for(int y=bl.y; y<=tr.y; y++)
                        SceneManager.UnloadSceneAsync(bl.x+","+y);
                    
                    bl.x++;
                    tr.x++;

                    for(int y=bl.y; y<=tr.y; y++)
                        SceneManager.LoadSceneAsync(tr.x+","+y, LoadSceneMode.Additive);

                } else if((currPos.x-1)/3 < bl.x) {
                    // load to left

                    for(int y=bl.y; y<=tr.y; y++)
                        SceneManager.UnloadSceneAsync(tr.x+","+y);
                    
                    tr.x--;
                    bl.x--;
                    
                    for(int y=bl.y; y<=tr.y; y++)
                        SceneManager.LoadSceneAsync(bl.x+","+y, LoadSceneMode.Additive);

                } else if((currPos.y+1)/3 > tr.y) {
                    // load up

                    for(int x=bl.x; x<=tr.x; x++)
                        SceneManager.UnloadSceneAsync(x+","+bl.y);
                    
                    bl.y++;
                    tr.y++;
                    
                    for(int x=bl.x; x<=tr.x; x++)
                        SceneManager.LoadSceneAsync(x+","+tr.y, LoadSceneMode.Additive);

                } else if((currPos.y-1)/3 < bl.y) {
                    // load down

                    for(int x=bl.x; x<=tr.x; x++)
                        SceneManager.UnloadSceneAsync(x+","+tr.y);
                    
                    tr.y--;
                    bl.y--;

                    for(int x=bl.x; x<=tr.x; x++)
                        SceneManager.LoadSceneAsync(x+","+bl.y, LoadSceneMode.Additive);
                }
            }
             catch {}

            print(currPos);
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;

        if(Input.GetKeyDown(KeyCode.Keypad9)) {
            SceneManager.LoadSceneAsync("2,0", LoadSceneMode.Additive);
        }
    }
}
