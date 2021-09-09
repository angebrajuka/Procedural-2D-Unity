using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(_EditorMapTextureGen))]
public class EditorMapTextureGen : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (_EditorMapTextureGen)target;

        if(GUILayout.Button("generate"))
        {
            script.proceduralGeneration.Init();
            if(script.randSeed)
            {
                ProceduralGeneration.SetSeed(ProceduralGeneration.RandomSeed());
                script.seed = ProceduralGeneration.seed_main;
            }
            else
            {
                ProceduralGeneration.SetSeed(script.seed);
            }
            script.proceduralGeneration.GenerateMap();
            ProceduralGeneration.GenerateTexture(script.resolution);
        }
        if(GUILayout.Button("clear"))
        {
            ProceduralGeneration.Clear();
        }

        if(ProceduralGeneration.textureBiome != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), ProceduralGeneration.textureBiome, null, ScaleMode.ScaleToFit);
        if(ProceduralGeneration.textureDecor != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), ProceduralGeneration.textureDecor, null, ScaleMode.ScaleToFit);
    }
}
