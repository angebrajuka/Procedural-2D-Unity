using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(_EditorTextureGen))]
public class EditorTextureGen : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (_EditorTextureGen)target;

        if(GUILayout.Button("generate texture"))
        {
            script.GenerateTexture();
        }
    }
}
