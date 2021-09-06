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

    public void Generate()
    {
        proceduralGeneration.Init(seed);
        proceduralGeneration.GenerateMap();
        texture = proceduralGeneration.mapTexture;
    }
}

#endif