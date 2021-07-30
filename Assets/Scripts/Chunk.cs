﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class Chunk : MonoBehaviour
{
    public int x, y;
    public DynamicLoading dynamicLoader;
    
    Tilemap m_tilemap;
    bool isValid;
    Vector3Int pos;
    string[] txt;
    ResourceRequest request;
    bool loaded;

    public void _Start(bool editor)
    {
        loaded = false;
        isValid = DynamicLoading.IsValid(x, y);
        m_tilemap = transform.GetChild(0).GetComponent<Tilemap>();
        pos = new Vector3Int(0, 0, 0);

        if(isValid)
        {
            if(editor)
            {
                txt = ((TextAsset)Resources.Load("Chunks/"+DynamicLoading.Name(x, y), typeof(TextAsset))).text.Split(',');
            }
            else
            {
                request = Resources.LoadAsync("Chunks/"+DynamicLoading.Name(x, y), typeof(TextAsset));
            }
        }
    }

    void Start()
    {
        _Start(false);
    }
    
    public void FixedUpdate()
    {
        if(request != null && request.isDone && txt == null)
        {
            txt = ((TextAsset)(request.asset)).text.Split(',');
        }
        if(request == null || txt != null)
        {
            for(pos.y=0; pos.y<=DynamicLoading.chunkSize; pos.y++)
            {
                if(pos.x < DynamicLoading.chunkSize && pos.y < DynamicLoading.chunkSize)
                {
                    int tile = 0;
                    if(isValid)
                    {
                        tile = Int32.Parse(txt[pos.x*DynamicLoading.chunkSize + pos.y]);
                    }
                    m_tilemap.SetTile(pos, dynamicLoader.tiles[tile]);
                }
                if(pos.x > 0 && pos.y > 0)
                {
                    m_tilemap.RefreshTile(pos+Vector3Int.left+Vector3Int.down);
                }
            }
            pos.x++;
            
            if(pos.x == DynamicLoading.chunkSize+1)
            {
                loaded = true;
                enabled = false;
            }
        }
    }
}
