using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

[CustomEditor(typeof(_EditorMapTextureGen))]
public class EditorMapTextureGen : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (_EditorMapTextureGen)target;

        if(GUILayout.Button("generate"))
        {
            script.worldGen.Start();
            if(script.randSeed)
            {
                WorldGen.SetSeed(WorldGen.RandomSeed());
                script.seed = WorldGen.seed;
            }
            else
            {
                WorldGen.SetSeed(script.seed);
            }
            script.worldGen.GenerateMapLagSpike();
            WorldGen.GenerateTexture(script.resolution);
        }
        if(GUILayout.Button("clear"))
        {
            script.worldGen.Clear();
        }

        if(WorldGen.textureBiome != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), WorldGen.textureBiome, null, ScaleMode.ScaleToFit);
        if(WorldGen.textureDecor != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), WorldGen.textureDecor, null, ScaleMode.ScaleToFit);
        if(WorldGen.textureDungeons != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), WorldGen.textureDungeons, null, ScaleMode.ScaleToFit);
    }
}
