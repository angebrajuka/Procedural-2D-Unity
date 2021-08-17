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
        dynamicLoader.Init();
        loadedChunk = dynamicLoader.InstantiateChunk(x, y);
        loadedChunk.transform.SetParent(transform);
        Chunk chunk = loadedChunk.GetComponent<Chunk>();
        chunk.x = x;
        chunk.y = y;
        chunk._Start(true);
        for(int i=0; i<DynamicLoading.chunkSize; i++)
        {
            chunk.Update();
        }

        chunk.m_tilemap.gameObject.name = "Ground "+x+","+y;
    }

    public void Save()
    {
        dynamicLoader.Init();

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
                file.Write(Array.IndexOf(DynamicLoading.tiles, tilemap.GetTile(pos))+"");
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

    public void LoadAll()
    {
        string[] files = Directory.GetFiles(Application.dataPath+"/Resources/Chunks", "*.txt");
        foreach(var name in files)
        {
            string pair = name.Substring(name.LastIndexOf('\\')+1, name.LastIndexOf('.')-name.LastIndexOf('\\')-1);
            int x = Int32.Parse(pair.Substring(0, pair.LastIndexOf(',')));
            int y = Int32.Parse(pair.Substring(pair.LastIndexOf(',')+1));
            
            this.x = x;
            this.y = y;
            Load();
        }
    }

    public void SaveAll()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Chunk chunk = child.GetComponent<Chunk>();
            x = chunk.x;
            y = chunk.y;
            loadedChunk = child.gameObject;
            Save();
        }
    }
}

#endif