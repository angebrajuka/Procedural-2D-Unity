#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class _EditorMapTextureGen : MonoBehaviour
{
    public Texture2D texture;
    public float seed;

    public void Generate()
    {
        Color land = new Color(0.5f, 0.9f, 0.5f);
        Color water = new Color(0.2f, 0.5f, 0.9f);

        ProceduralGeneration.SetSeed(seed);

        var width = ProceduralGeneration.mapDiameter*ProceduralGeneration.chunkSize;
        texture = new Texture2D(width, width);

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<width; y++)
            {
                texture.SetPixel(x, y, ProceduralChunk.PerlinMain(new Vector2Int(x, y)) == 5 ? land : water);
            }
        }
        texture.Apply();

        AssetDatabase.CreateAsset(texture, "Assets/EditorGeneratedResources/mapTexture.asset");
    }
}

#endif