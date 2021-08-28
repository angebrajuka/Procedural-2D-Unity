using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralChunkUnloader : MonoBehaviour
{
    public Vector3Int pos;
    Vector3Int tilePos = new Vector3Int(0, 0, 0);

    void Start()
    {
        if(ProceduralGeneration.loadedChunks.Contains((pos.x, pos.y)))
        {
            ProceduralGeneration.loadedChunks.Remove((pos.x, pos.y));
        }
    }

    void Update()
    {
        for(tilePos.y=0; tilePos.y<DynamicLoading.chunkSize; tilePos.y++)
        {
            if(tilePos.x < DynamicLoading.chunkSize && tilePos.y < DynamicLoading.chunkSize)
            {
                ProceduralGeneration.instance.tilemap_ground.SetTile(tilePos+pos*ProceduralGeneration.chunkSize, ProceduralGeneration.tiles[1]);
            }
        }
        tilePos.x++;
        
        if(tilePos.x == DynamicLoading.chunkSize+1)
        {
            // ProceduralGeneration.instance.CheckLoaded(this);
        }
    }
}
