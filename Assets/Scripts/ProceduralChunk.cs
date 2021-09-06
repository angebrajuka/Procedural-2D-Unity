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

    const int perlinOffset = 5429; // prevents mirroring

    // returns 0 for water, 1 for shallow water, 2 for sand, 3 for biome gen
    public static int PerlinMain(Vector2Int pos)
    {
        const float perlinScale = 0.01f;
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
        return val > landVal ? 3 : val > sandVal ? 2 : val > shallowWaterVal ? 1 : 0;
    }

    public static int PerlinBiome(Vector2Int pos)
    {
        const float perlinScaleRain = 0.003f;
        const float perlinScaleTemp = 0.003f;

        float perlinValRain = Mathf.PerlinNoise((ProceduralGeneration.seed_rain + pos.x + perlinOffset)*perlinScaleRain, (ProceduralGeneration.seed_rain + pos.y + perlinOffset)*perlinScaleRain);
        float perlinValTemp = Mathf.PerlinNoise((ProceduralGeneration.seed_temp + pos.x + perlinOffset)*perlinScaleTemp, (ProceduralGeneration.seed_temp + pos.y + perlinOffset)*perlinScaleTemp);

        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((ProceduralGeneration.seed_main + pos.x + perlinOffset)*perlinScaleFine, (ProceduralGeneration.seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);

        perlinValTemp -= fineNoise;
        perlinValTemp = Mathf.Clamp(Mathf.Round(perlinValTemp * ProceduralGeneration.tex_map_width), 0, ProceduralGeneration.tex_map_width-1);
        perlinValRain -= fineNoise;
        perlinValRain = Mathf.Clamp(Mathf.Round(perlinValRain * ProceduralGeneration.tex_map_width), 0, perlinValTemp);

        return ProceduralGeneration.rain_temp_map[(int)perlinValTemp, (int)perlinValRain];
    }

    Vector2Int pos = new Vector2Int(0, 0);
    void Update()
    {
        int i=0;
        for(tilePos.y=-1; tilePos.y<=DynamicLoading.chunkSize; tilePos.y++, i++)
        {
            positionArray[i] = tilePos;
            pos.Set((int)transform.localPosition.x+tilePos.x, (int)transform.localPosition.y+tilePos.y);
            int perlinMain = PerlinMain(pos);
            tileArray[i] = perlinMain == 3 ? ProceduralGeneration.biomes[PerlinBiome(pos)].tile : tiles[perlinMain];
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
