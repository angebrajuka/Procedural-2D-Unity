using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tool_KeyCode))]
public class Editor_GetKeyCode : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (Tool_KeyCode)target;

        if(GUILayout.Button("get"))
        {
            script.Get();
        }
    }
}