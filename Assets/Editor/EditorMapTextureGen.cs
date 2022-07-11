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
                script.worldGen.SetSeed(script.worldGen.RandomSeed());
                script.seed = script.worldGen.seed;
            }
            else
            {
                script.worldGen.SetSeed(script.seed);
            }
            script.worldGen.GenerateMap();
            script.worldGen.GenerateTexture(script.resolution);
        }
        if(GUILayout.Button("clear"))
        {
            script.worldGen.Clear();
        }

        if(script.worldGen.textureBiome != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), script.worldGen.textureBiome, null, ScaleMode.ScaleToFit);
        if(script.worldGen.textureDecor != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), script.worldGen.textureDecor, null, ScaleMode.ScaleToFit);
        if(script.worldGen.textureDungeons != null) EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), script.worldGen.textureDungeons, null, ScaleMode.ScaleToFit);
    }
}
