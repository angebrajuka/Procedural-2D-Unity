using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLoading : MonoBehaviour {

    [HideInInspector]
    public Rigidbody2D player_rb;
    private Vector2Int currPos=Vector2Int.zero;
    private Vector2Int prevPos=Vector2Int.zero;
    
    void Start() {
        player_rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        currPos.x = (int)(player_rb.position.x/100);
        currPos.y = (int)(player_rb.position.y/100);

        
        if(currPos != prevPos) {
            print(currPos);
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;
    }
}
