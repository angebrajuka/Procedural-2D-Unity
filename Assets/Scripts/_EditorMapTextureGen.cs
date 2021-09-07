#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static ProceduralGeneration;

public class _EditorMapTextureGen : MonoBehaviour
{
    public ProceduralGeneration proceduralGeneration;
    public Texture2D textureBiome;
    public Texture2D textureDecor;
    public float seed;

    Color32 AverageColorFromTexture(Texture2D tex)
    {
        Color32[] texColors = tex.GetPixels32();

        int total = texColors.Length;
        float r = 0;
        float g = 0;
        float b = 0;

        for(int i = 0; i < total; i++)
        {
            r += texColors[i].r;
            g += texColors[i].g;
            b += texColors[i].b;
        }

        return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 0);
    }

    public void Generate()
    {
        proceduralGeneration.Init();
        SetSeed(seed);
        proceduralGeneration.GenerateMap();
        
        var colors = new Color32[tiles.Length];
        for(int i=0; i<tiles.Length; i++)
        {
            colors[i] = AverageColorFromTexture(tiles[i].m_DefaultSprite.texture);
        }
        
        textureBiome = new Texture2D(mapDiameter*chunkSize, mapDiameter*chunkSize);
        for(int x=0; x<textureBiome.width; x++)
        {
            for(int y=0; y<textureBiome.height; y++)
            {
                textureBiome.SetPixel(x, y, colors[mapTexture_biome[x, y]]);
            }
        }
        textureBiome.Apply();
        
        textureDecor = new Texture2D(mapDiameter*chunkSize, mapDiameter*chunkSize);
        for(int x=0; x<textureBiome.width; x++)
        {
            for(int y=0; y<textureBiome.height; y++)
            {
                textureDecor.SetPixel(x, y, mapTexture_decor[x, y] == 0 ? Color.white : Color.black);
            }
        }
        textureDecor.Apply();
    }
}

#endif