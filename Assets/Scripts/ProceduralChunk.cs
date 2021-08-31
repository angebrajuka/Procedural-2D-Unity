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


    // returns true for land, false for water
    public static bool PerlinMain(Vector2Int pos)
    {
        float perlinVal = Mathf.PerlinNoise(ProceduralGeneration.seed_main + pos.x/20.0f+5429, ProceduralGeneration.seed_main + pos.y/20.0f+5429)/2+0.5f; // 0.5 to 1
        float gradientVal = 1-Vector2Int.Distance(pos, ProceduralGeneration.center)/(ProceduralGeneration.chunkSize*ProceduralGeneration.mapRadius); // 1 in center, 0 at edge of map

        return (perlinVal+gradientVal)/2 > 0.5;
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
                    m_tilemap.SetTile(tilePos, PerlinMain(new Vector2Int((int)transform.localPosition.x+tilePos.x, (int)transform.localPosition.y+tilePos.y)) ? ProceduralGeneration.instance.grass : ProceduralGeneration.instance.water);
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
