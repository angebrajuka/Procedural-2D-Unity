using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class StringFloat {
    public string s;
    public float f;
}

[System.Serializable]
public class Decoration {
    public RuleTile tile;
    public float threshhold;
    public bool collider;
    public Vector2Int Size { get { return new Vector2Int((int)Mathf.Round(tile.m_DefaultSprite.texture.width/16), (int)Mathf.Round(tile.m_DefaultSprite.texture.height/32)); } }
}

[System.Serializable]
public class Biome {
    public string biome_name;
    public RuleTile tile;
    public Color32 rain_temp_map_color;
    public Decoration[] decorations;

    public void Init() {
        for(int i=1; i<decorations.Length; i++) {
            decorations[i].threshhold += decorations[i-1].threshhold;
        }
    }
}