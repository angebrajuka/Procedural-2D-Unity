using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralChunk : MonoBehaviour
{
    public Tilemap m_tilemap;
    Vector3Int tilePos;

    bool refreshing;
    bool removing;
    public bool loaded;

    public void _Start()
    {
        m_tilemap = transform.GetChild(0).GetComponent<Tilemap>();
    }


    public void Init()
    {
        loaded = false;
        refreshing = false;
        tilePos = new Vector3Int(0, 0, 0);
    }


    float PerlinMain(Vector3Int pos)
    {
        pos += new Vector3Int((int)transform.localPosition.x, (int)transform.localPosition.y, 0);

        float perlinVal = Mathf.PerlinNoise(ProceduralGeneration.seed_main + pos.x, ProceduralGeneration.seed_main + pos.y);
        float gradientVal = Vector3Int.Distance(pos, ProceduralGeneration.center*ProceduralGeneration.chunkSize)/ProceduralGeneration.chunkSize;

        return (perlinVal-gradientVal+1);
    }

    void Update()
    {
        if(removing)
        {
            
        }
        else
        {
            for(tilePos.y=0; tilePos.y<DynamicLoading.chunkSize; tilePos.y++)
            {
                if(refreshing)
                {
                    m_tilemap.RefreshTile(tilePos);
                }
                else
                {
                    m_tilemap.SetTile(tilePos, PerlinMain(tilePos) > 0.1f ? ProceduralGeneration.instance.grass : ProceduralGeneration.instance.water);
                }
            }
            tilePos.x++;

            if(tilePos.x == DynamicLoading.chunkSize+1)
            {
                if(refreshing)
                {
                    m_tilemap.CompressBounds();
                    loaded = true;
                    ProceduralGeneration.instance.CheckLoaded();
                    enabled = false;
                }
                else
                {
                    refreshing = true;
                    tilePos.x = 0;
                }
            }
        }
    }
}
