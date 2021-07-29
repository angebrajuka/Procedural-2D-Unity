#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Tilemaps;
using UnityEditor;

[ExecuteInEditMode]
public class _EditorChunkLoading : MonoBehaviour
{
    public DynamicLoading dynamicLoader;
    public int x, y;
    public GameObject loadedChunk;

    public void Load()
    {
        dynamicLoader.Start();
        loadedChunk = dynamicLoader.InstantiateChunk(x, y);
        dynamicLoader.LoadChunk(x, y, loadedChunk);
    }

    public void Save()
    {
        StreamWriter file;
        string path = Application.dataPath+"/Resources/Chunks/"+DynamicLoading.Name(x, y)+".txt";
        if(File.Exists(path))
        {
            File.Delete(path);
        }
        file = new StreamWriter(File.Create(path));

        var tilemap = loadedChunk.transform.GetChild(0).GetComponent<Tilemap>();
        var pos = new Vector3Int(0, 0, 0);
        for(pos.x=0; pos.x<DynamicLoading.chunkSize; pos.x++)
        {
            for(pos.y=0; pos.y<DynamicLoading.chunkSize; pos.y++)
            {
                file.Write(Array.IndexOf(dynamicLoader.tiles, tilemap.GetTile(pos))+"");
                if(pos.x != DynamicLoading.chunkSize-1 || pos.y != DynamicLoading.chunkSize-1) file.Write(",");
            }
        }
        file.Close();
        AssetDatabase.Refresh();
    }

    public void GetValidChunks()
    {
        string path = Application.dataPath+"/Resources/ValidChunks.txt";
        File.Delete(path);
        File.Delete(path+".meta");
        var file = new StreamWriter(File.Create(path));
        string[] files = Directory.GetFiles(Application.dataPath+"/Resources/Chunks", "*.txt");
        foreach(var name in files)
        {
            file.Write(name.Substring(name.LastIndexOf('\\')+1, name.LastIndexOf('.')-name.LastIndexOf('\\')-1)+";");
        }
        file.Close();
        AssetDatabase.Refresh();
    }
}

#endif