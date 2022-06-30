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
            script.proceduralGeneration.Start();
            if(script.randSeed)
            {
                WorldGen.SetSeed(WorldGen.RandomSeed());
                script.seed = WorldGen.seed;
            }
            else
            {
                WorldGen.SetSeed(script.seed);
            }
            script.proceduralGeneration.GenerateMap();
            WorldGen.GenerateTexture(script.resolution);
        }
        if(GUILayout.Button("clear"))
        {
            WorldGen.Clear();
        }

        if(WorldGen.textureBiome != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), WorldGen.textureBiome, null, ScaleMode.ScaleToFit);
        if(WorldGen.textureDecor != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), WorldGen.textureDecor, null, ScaleMode.ScaleToFit);
    }
}
