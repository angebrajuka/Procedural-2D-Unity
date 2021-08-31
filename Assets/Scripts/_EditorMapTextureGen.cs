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
        ProceduralGeneration.SetSeed(seed);

        var width = ProceduralGeneration.mapDiameter*ProceduralGeneration.chunkSize;
        texture = new Texture2D(width, width);

        for(int x=0; x<width; x++)
        {
            for(int y=0; y<width; y++)
            {
                texture.SetPixel(x, y, ProceduralChunk.PerlinMain(new Vector2Int(x, y)) ? Color.black : Color.white);
            }
        }
        texture.Apply();

        AssetDatabase.CreateAsset(texture, "Assets/EditorGeneratedResources/mapTexture.asset");
    }
}
