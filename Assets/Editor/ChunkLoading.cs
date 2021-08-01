using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(_EditorChunkLoading))]
public class ChunkLoading : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (_EditorChunkLoading)target;

        if(GUILayout.Button("new/load chunk"))
        {
            script.Load();
        }
        if(GUILayout.Button("save chunk"))
        {
            script.Save();
        }
        if(GUILayout.Button("get valid chunks"))
        {
            script.GetValidChunks();
        }
        if(GUILayout.Button("load all"))
        {
            script.LoadAll();
        }
        if(GUILayout.Button("save all"))
        {
            script.SaveAll();
        }
    }
}
