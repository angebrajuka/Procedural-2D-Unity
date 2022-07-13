using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System;

[CustomEditor(typeof(_EditorMapTextureGen))]
public class EditorMapTextureGen : Editor {
    Texture2D[] textures;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var script = (_EditorMapTextureGen)target;

        if(GUILayout.Button("generate")) {
            script.worldGen.Start();
            if(script.randSeed) {
                script.seed = script.worldGen.RandomSeed();
            }
            var tex = script.worldGen.GenerateTextures(script.resolution, script.worldGen.GenerateWorld(script.seed));
            textures = new Texture2D[tex.textures_overworld.Length + tex.textures_dungeons.Length];
            Array.Copy(tex.textures_overworld, textures, tex.textures_overworld.Length);
            Array.Copy(tex.textures_dungeons, 0, textures, tex.textures_overworld.Length, tex.textures_dungeons.Length);
        }
        if(GUILayout.Button("clear")) {
            textures = null;
        }

        if(textures != null) {
            foreach(var texture in textures) {
                EditorGUI.DrawPreviewTexture(
                    GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth),
                    texture,
                    null,
                    ScaleMode.ScaleToFit
                );
            }
        }
    }
}
