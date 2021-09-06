using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralChunk : MonoBehaviour
{
    public Tilemap m_tilemap;
    Vector3Int tilePos;
    public bool loaded;
    Vector3Int[] positionArray=new Vector3Int[ProceduralGeneration.chunkSize];
    TileBase[] tileArray=new TileBase[ProceduralGeneration.chunkSize];

    public void _Start()
    {
        m_tilemap = transform.GetChild(0).GetComponent<Tilemap>();
    }

    
    public void Init()
    {
        loaded = false;
        tilePos = new Vector3Int(0, 0, 0);
    }


    Vector2Int pos = new Vector2Int(0, 0);
    void Update()
    {
        int i=0;
        for(tilePos.y=0; tilePos.y<DynamicLoading.chunkSize; tilePos.y++, i++)
        {
            positionArray[i] = tilePos;
            tileArray[i] = ProceduralGeneration.GetTile((int)transform.localPosition.x+tilePos.x, (int)transform.localPosition.y+tilePos.y);
        }
        m_tilemap.SetTiles(positionArray, tileArray);
        tilePos.x++;

        if(tilePos.x == DynamicLoading.chunkSize)
        {
            m_tilemap.CompressBounds();
            loaded = true;
            ProceduralGeneration.instance.CheckLoaded();
            enabled = false;
        }
    }
}
