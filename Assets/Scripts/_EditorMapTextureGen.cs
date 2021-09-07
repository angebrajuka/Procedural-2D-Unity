#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        ProceduralGeneration.SetSeed(seed);
        proceduralGeneration.GenerateMap();
        
        var colors = new Color32[ProceduralGeneration.tiles.Length];
        for(int i=0; i<ProceduralGeneration.tiles.Length; i++)
        {
            colors[i] = AverageColorFromTexture(ProceduralGeneration.tiles[i].m_DefaultSprite.texture);
        }
        
        textureBiome = new Texture2D(proceduralGeneration.mapTexture_biome.width, proceduralGeneration.mapTexture_biome.height);
        for(int x=0; x<textureBiome.width; x++)
        {
            for(int y=0; y<textureBiome.height; y++)
            {
                Color32 c = proceduralGeneration.mapTexture_biome.GetPixel(x, y);
                textureBiome.SetPixel(x, y, colors[c.r]);
            }
        }
        textureBiome.Apply();
        
        textureDecor = new Texture2D(proceduralGeneration.mapTexture_decor.width, proceduralGeneration.mapTexture_decor.height);
        for(int x=0; x<textureBiome.width; x++)
        {
            for(int y=0; y<textureBiome.height; y++)
            {
                Color32 c = proceduralGeneration.mapTexture_decor.GetPixel(x, y);
                textureDecor.SetPixel(x, y, c.r == 0 ? Color.white : Color.black);
            }
        }
        textureDecor.Apply();
    }
}

#endif