using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(_EditorGetCollider))]
public class EditorGetCollider : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (_EditorGetCollider)target;

        if(GUILayout.Button("get"))
        {
            script.Get();
        }
    }
}
