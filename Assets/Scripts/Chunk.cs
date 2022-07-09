using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using static Singles;

public class Chunk : MonoBehaviour {
    // hierarchy
    public Tilemap m_tilemap;
    public Transform decorations;

    Vector3Int tilePos;
    Vector3Int[] positionArray=new Vector3Int[WorldGen.chunkSize+3];
    TileBase[] tileArray=new TileBase[WorldGen.chunkSize+3];
    [HideInInspector] public bool loaded;

    public void _Start() {
        m_tilemap = transform.GetChild(0).GetComponent<Tilemap>();
        enabled = false;
    }
    
    public void Init() {
        loaded = false;
        tilePos = new Vector3Int(-1, 0, 0);
    }

    public void RemoveDecorations() {
        while(decorations.childCount > 0) {
            Transform child = decorations.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }
    }

    public void Update() {
        int i=0;
        for(tilePos.y=-2; tilePos.y<=WorldGen.chunkSize; tilePos.y++, i++) {
            positionArray[i] = tilePos;
            int x = (int)transform.localPosition.x+tilePos.x;
            int y = (int)transform.localPosition.y+tilePos.y;
            int tile = singles.worldGen.GetTile(x, y);
            tileArray[i] = WorldGen.tiles[tile];
            singles.worldGen.SpawnDecoration(x, y, tile, decorations);
        }
        m_tilemap.SetTiles(positionArray, tileArray);
        tilePos.x++;

        if(tilePos.x == WorldGen.chunkSize+1) {
            m_tilemap.CompressBounds();
            loaded = true;
            enabled = false;
        }
    }
}
