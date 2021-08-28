using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralChunkLoader : MonoBehaviour
{
    public Vector3Int pos;
    Vector3Int tilePos = new Vector3Int(0, 0, 0);

    float Perlin(Vector2Int pos)
    {
        float perlinVal = Mathf.PerlinNoise(ProceduralGeneration.seed + pos.x, ProceduralGeneration.seed + pos.y);
        float gradientVal = Vector2Int.Distance(pos, ProceduralGeneration.center)/ProceduralGeneration.mapRadius;

        return (perlinVal-gradientVal+1)/2;
    }

    void Update()
    {
        for(tilePos.y=0; tilePos.y<=DynamicLoading.chunkSize; tilePos.y++)
        {
            if(tilePos.x < DynamicLoading.chunkSize && tilePos.y < DynamicLoading.chunkSize)
            {
                ProceduralGeneration.instance.tilemap_ground.SetTile(tilePos+pos*ProceduralGeneration.chunkSize, ProceduralGeneration.tiles[1]);
            }
            if(tilePos.x > 0 && tilePos.y > 0)
            {
                ProceduralGeneration.instance.tilemap_ground.RefreshTile(tilePos+Vector3Int.left+Vector3Int.down);
            }
        }
        tilePos.x++;
        
        if(tilePos.x == DynamicLoading.chunkSize+1)
        {
            ProceduralGeneration.instance.CheckLoaded(this);
        }
    }
}
