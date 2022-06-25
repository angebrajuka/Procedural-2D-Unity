using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralChunk : MonoBehaviour
{
    // hierarchy
    public Tilemap m_tilemap;
    public Transform decorations;

    Vector3Int tilePos;
    Vector3Int[] positionArray=new Vector3Int[ProceduralGeneration.chunkSize];
    TileBase[] tileArray=new TileBase[ProceduralGeneration.chunkSize];
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
        for(tilePos.y=0; tilePos.y<ProceduralGeneration.chunkSize; tilePos.y++, i++)
        {
            positionArray[i] = tilePos;
            int x = (int)transform.localPosition.x+tilePos.x;
            int y = (int)transform.localPosition.y+tilePos.y;
            int tile = ProceduralGeneration.GetTile(x, y);
            tileArray[i] = ProceduralGeneration.tiles[tile];
            ProceduralGeneration.SpawnDecoration(x, y, tile, decorations);
        }
        m_tilemap.SetTiles(positionArray, tileArray);
        tilePos.x++;

        if(tilePos.x == ProceduralGeneration.chunkSize)
        {
            m_tilemap.CompressBounds();
            loaded = true;
            ProceduralGeneration.instance.CheckLoaded();
            enabled = false;
        }
    }
}
