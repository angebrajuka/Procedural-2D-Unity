#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class _EditorMapTextureGen : MonoBehaviour
{
    public ProceduralGeneration proceduralGeneration;
    public Texture2D texture;
    public float seed;

    public Color[] colors;
    public Color water;
    public Color water_shallow;
    public Color beach;

    public void Generate()
    {
        proceduralGeneration.Init(seed);

        Color[] beachColors = new Color[]{water, water_shallow, beach};

        var width = ProceduralGeneration.mapDiameter*ProceduralGeneration.chunkSize;
        texture = new Texture2D(width, width);

        Vector2Int pos = new Vector2Int(0, 0);
        for(pos.x=0; pos.x<width; pos.x++)
        {
            for(pos.y=0; pos.y<width; pos.y++)
            {
                int val = ProceduralChunk.PerlinMain(pos);
                texture.SetPixel(pos.x, pos.y, val == 3 ? colors[ProceduralChunk.PerlinBiome(pos)] : beachColors[val]);
            }
        }
        texture.Apply();

        AssetDatabase.CreateAsset(texture, "Assets/EditorGeneratedResources/mapTexture.asset");
    }
}

#endif