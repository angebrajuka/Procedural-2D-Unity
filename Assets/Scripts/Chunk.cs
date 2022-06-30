using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using static Singles;

public class Chunk : MonoBehaviour
{
    // hierarchy
    public Tilemap m_tilemap;
    public Transform decorations;

    Vector3Int tilePos;
    Vector3Int[] positionArray=new Vector3Int[WorldGen.chunkSize];
    TileBase[] tileArray=new TileBase[WorldGen.chunkSize];
    [HideInInspector] public bool loaded;

    public void _Start()
    {
        m_tilemap = transform.GetChild(0).GetComponent<Tilemap>();
        enabled = false;
    }
    
    public void Init()
    {
        loaded = false;
        tilePos = new Vector3Int(0, 0, 0);
    }

    public void RemoveDecorations()
    {
        while(decorations.childCount > 0)
        {
            Transform child = decorations.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }
    }

    void Update()
    {
        int i=0;
        for(tilePos.y=0; tilePos.y<WorldGen.chunkSize; tilePos.y++, i++)
        {
            positionArray[i] = tilePos;
            int x = (int)transform.localPosition.x+tilePos.x;
            int y = (int)transform.localPosition.y+tilePos.y;
            int tile = WorldGen.GetTile(x, y);
            tileArray[i] = WorldGen.tiles[tile];
            WorldGen.SpawnDecoration(x, y, tile, decorations);
        }
        m_tilemap.SetTiles(positionArray, tileArray);
        tilePos.x++;

        if(tilePos.x == WorldGen.chunkSize)
        {
            m_tilemap.CompressBounds();
            loaded = true;
            singles.worldGen.CheckLoaded();
            enabled = false;
        }
    }
}
