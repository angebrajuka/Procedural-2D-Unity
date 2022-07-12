using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Campfire))]
public class EditorCampfire : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var script = (Campfire)target;

        script.Lit = GUILayout.Toggle(script.Lit, "active");
    }
}
