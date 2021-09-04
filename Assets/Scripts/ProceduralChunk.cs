using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralChunk : MonoBehaviour
{
    public Tilemap m_tilemap;
    Vector3Int tilePos;
    public bool loaded;
    TileBase[] tiles = new TileBase[3];
    Vector3Int[] positionArray=new Vector3Int[ProceduralGeneration.chunkSize+2];
    TileBase[] tileArray=new TileBase[ProceduralGeneration.chunkSize+2];

    public void _Start()
    {
        m_tilemap = transform.GetChild(0).GetComponent<Tilemap>();
        tiles[0] = ProceduralGeneration.tile_water;
        tiles[1] = ProceduralGeneration.tile_water_shallow;
        tiles[2] = ProceduralGeneration.tile_sand;
    }

    
    public void Init()
    {
        loaded = false;
        tilePos = new Vector3Int(-1, 0, 0);
    }


    // returns 0 for water, 1 for shallow water, 2 for sand, 3 for biome gen
    public static int PerlinMain(Vector2Int pos)
    {
        const float perlinScale = 0.01f;
        const int perlinOffset = 5429; // prevents mirroring
        float perlinVal = Mathf.PerlinNoise((ProceduralGeneration.seed_main + pos.x + perlinOffset)*perlinScale, (ProceduralGeneration.seed_main + pos.y + perlinOffset)*perlinScale); // 0 to 1
        perlinVal = Math.Remap(perlinVal, 0, 1, 0.3f, 1);
        float gradientVal = 1-Vector2Int.Distance(pos, ProceduralGeneration.center)/(ProceduralGeneration.chunkSize*ProceduralGeneration.mapRadius); // 1 in center, 0 at edge of map
        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((ProceduralGeneration.seed_main + pos.x + perlinOffset)*perlinScaleFine, (ProceduralGeneration.seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);
        
        const float landVal = 0.52f;
        const float sandVal = 0.5f;
        const float shallowWaterVal = 0.48f;

        float val = (perlinVal+gradientVal)/2-fineNoise;
        return val > landVal ? 3 : val > sandVal ? 2 : val > shallowWaterVal ? 1 : 0;//(int)Mathf.Round(Math.Remap(val, sandVal, landVal, 0, 4)) : val > shallowWaterVal ? (int)Mathf.Round(Math.Remap(val, shallowWaterVal, sandVal, )) : 0;
    }

    public static TileBase PerlinBiome(Vector2Int pos)
    {
        return ProceduralGeneration.tiles_land[1];
    }

    void Update()
    {
        int i=0;
        for(tilePos.y=-1; tilePos.y<=DynamicLoading.chunkSize; tilePos.y++, i++)
        {
            positionArray[i] = tilePos;
            var pos = new Vector2Int((int)transform.localPosition.x+tilePos.x, (int)transform.localPosition.y+tilePos.y);
            int perlinMain = PerlinMain(pos);
            tileArray[i] = perlinMain == 3 ? PerlinBiome(pos) : tiles[perlinMain];
        }
        m_tilemap.SetTiles(positionArray, tileArray);
        tilePos.x++;

        if(tilePos.x == DynamicLoading.chunkSize+1)
        {
            m_tilemap.CompressBounds();
            loaded = true;
            ProceduralGeneration.instance.CheckLoaded();
            enabled = false;
        }
    }
}
