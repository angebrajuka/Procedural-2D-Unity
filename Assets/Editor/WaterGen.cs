using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(_EditorWaterGen))]
public class WaterGen : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (_EditorWaterGen)target;

        if(GUILayout.Button("generate"))
        {
            script.Generate();
        }
    }
}
