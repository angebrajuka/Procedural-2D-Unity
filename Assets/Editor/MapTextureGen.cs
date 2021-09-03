using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(_EditorMapTextureGen))]
public class MapTextureGen : Editor
{
    bool randSeed = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (_EditorMapTextureGen)target;

        randSeed = EditorGUILayout.Toggle("random seed", randSeed);

        if(GUILayout.Button("generate"))
        {
            if(randSeed)
            {
                script.seed = ProceduralGeneration.RandomSeed();
            }
            script.Generate();
        }

        EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth), script.texture, null, ScaleMode.ScaleToFit);
    }
}