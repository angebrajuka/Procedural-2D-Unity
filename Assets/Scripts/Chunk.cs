using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk : MonoBehaviour {
    WorldGen worldGen;
    Vector3Int worldPos;
    Vector3Int tilePos;
    // Vector3Int[] positionArray=null;//new Vector3Int[WorldGen.chunkSize+3];
    // TileBase[] tileArray=null;//new TileBase[WorldGen.chunkSize+3];
    [HideInInspector] public bool loaded;

    public void _Start(WorldGen worldGen) {
        // this.worldGen = worldGen;
        // enabled = false;
        // worldPos = Vector3Int.zero;
        // tilePos = Vector3Int.zero;
    }
    
    public void Init(int x, int y, int z) {
        // loaded = false;
        // worldPos.Set(x, y, z);
        // tilePos.Set(0, 0, 0);
    }

    public void Unload() {
        // RemoveDecorations();
        // // worldGen.tilemap.origin = ;
        // // worldGen.tilemap.size = ;
        // Debug.Log(worldGen.tilemap.origin);
        // Debug.Log(worldGen.tilemap.size);
        // worldGen.tilemap.ResizeBounds();
        // worldGen.tilemap.CompressBounds();
    }

    public void RemoveDecorations() {
        // while(transform.childCount > 0) {
        //     Transform child = transform.GetChild(0);
        //     child.parent = null;
        //     Destroy(child.gameObject);
        // }
    }

    public void Update() {
        // int i=0;
        // for(tilePos.y=0; tilePos.y<WorldGen.chunkSize; tilePos.y++, i++) {
        //     positionArray[i] = worldPos+tilePos;
        //     int x = worldPos.x+tilePos.x;
        //     int y = worldPos.y+tilePos.y;
        //     int tile = worldGen.GetTile(x, y);
        //     tileArray[i] = WorldGen.tiles[tile];
        //     worldGen.SpawnDecoration(x, y, tile, transform);
        // }
        // worldGen.tilemap.SetTiles(positionArray, tileArray);
        // tilePos.x++;

        // if(tilePos.x == WorldGen.chunkSize) {
        //     worldGen.tilemap.CompressBounds();
        //     loaded = true;
        //     enabled = false;
        // }
    }
}
