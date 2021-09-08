using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(_EditorWaterGen))]
public class EditorWaterGen : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (_EditorWaterGen)target;

        if(GUILayout.Button("generate"))
        {
            script.Generate();
        }
        GUILayout.TextArea("remember to re add water tile to Procedural Generation script and other rule tiles", new GUILayoutOption[]{});
    }
}
