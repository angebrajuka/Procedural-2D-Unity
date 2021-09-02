#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using static UnityEngine.Vector2Int;
using System.IO;

public class _EditorWaterGen : MonoBehaviour
{
    public Sprite[] water;
    
    public Sprite L, U, R, D, LU, UR, RD, DL, LR, UD, URD, RDL, DLU, LUR, LURD; // all edge combos
    public Sprite ul, ur, bl, br; // all corners


    bool Intersects(Vector2Int[] first, Vector2Int[] second)
    {
        foreach(var vec1 in first)
        {
            foreach(var vec2 in second)
            {
                if(vec1.x == vec2.x && vec1.y == vec2.y) return true;
            }
        }

        return false;
    }

    public void Generate()
    {
        var edges = new Dictionary<Vector2Int[], Sprite>()
        {
            {new Vector2Int[] { },                          null},
            {new Vector2Int[] { left },                     L},
            {new Vector2Int[] { up },                       U},
            {new Vector2Int[] { right },                    R},
            {new Vector2Int[] { down },                     D},
            {new Vector2Int[] { left, up },                 LU},
            {new Vector2Int[] { up, right },                UR},
            {new Vector2Int[] { right, down },              RD},
            {new Vector2Int[] { down, left },               DL},
            {new Vector2Int[] { left, right },              LR},
            {new Vector2Int[] { up, down },                 UD},
            {new Vector2Int[] { up, right, down },          URD},
            {new Vector2Int[] { right, down, left },        RDL},
            {new Vector2Int[] { down, left, up },           DLU},
            {new Vector2Int[] { left, up, right },          LUR},
            {new Vector2Int[] { left, up, right, down},     LURD},
        };

        var corners = new Dictionary<Vector2Int[], Sprite[]>()
        {
            {new Vector2Int[] { },                          new Sprite[]{ }},
            {new Vector2Int[] { left, up },                 new Sprite[]{ul}},
            {new Vector2Int[] { up, right },                new Sprite[]{ur}},
            {new Vector2Int[] { right, down },              new Sprite[]{br}},
            {new Vector2Int[] { down, left },               new Sprite[]{bl}},
            {new Vector2Int[] { left, up, right },          new Sprite[]{ul, ur}},
            {new Vector2Int[] { up, right, down },          new Sprite[]{ur, br}},
            {new Vector2Int[] { right, down, left },        new Sprite[]{br, bl}},
            {new Vector2Int[] { down, left, up },           new Sprite[]{bl, ul}}, //
            {new Vector2Int[] { left, up, right, down },    new Sprite[]{ul, ur, br}},
            {new Vector2Int[] { left, up, right, down },    new Sprite[]{ur, br, bl}},
            {new Vector2Int[] { left, up, right, down },    new Sprite[]{br, bl, ul}},
            {new Vector2Int[] { left, up, right, down },    new Sprite[]{bl, ul, ur}},
            {new Vector2Int[] { left, up, right, down },    new Sprite[]{ul, ur, br, bl}},
            {new Vector2Int[] { left, up, right, down },    new Sprite[]{ul, br}},
            {new Vector2Int[] { left, up, right, down },    new Sprite[]{ur, bl}},
        };

        // AdvancedRuleTile ruleTile = ScriptableObject.CreateInstance("AdvancedRuleTile") as AdvancedRuleTile;

        // ruleTile.m_DefaultSprite = water[0];
        // ruleTile.m_DefaultColliderType = Tile.ColliderType.None;


        // RuleTile.TilingRule rule = new RuleTile.TilingRule();
        // rule.m_Sprites = new Sprite[2];
        // rule.m_Sprites[0] = water[0];
        // rule.m_Sprites[1] = water[1];
        // Dictionary<Vector3Int, int> dict = rule.GetNeighbors();
        // rule.ApplyNeighbors(dict);
        // rule.m_ColliderType = Tile.ColliderType.None;
        // rule.m_Output = RuleTile.TilingRule.OutputSprite.Animation;
        // rule.m_MinAnimationSpeed = 2;
        // rule.m_MaxAnimationSpeed = 3;

        // ruleTile.m_TilingRules.Add(rule);

        int i=0;
        foreach(var edge in edges)
        {
            foreach(var corner in corners)
            {
                if(Intersects(edge.Key, corner.Key) || (edge.Key.Length == 0 && corner.Key.Length == 0)) continue;

                var texture = new Texture2D(32, 16);

                var pixels = water[0].texture.GetPixels(0, 0, 16, 16);
                texture.SetPixels(0, 0, 16, 16, pixels);
                pixels = water[1].texture.GetPixels(16, 0, 16, 16);
                texture.SetPixels(16, 0, 16, 16, pixels);

                for(int x=0; x<16; x++)
                {
                    for(int y=0; y<16; y++)
                    {
                        if(edge.Value != null)
                        {
                            Rect rect = edge.Value.textureRect;
                            Color c = edge.Value.texture.GetPixel(x+(int)rect.x, y+(int)rect.y);
                            if(c.a != 0) texture.SetPixel(x, y, c);
                        }

                        foreach(var sprite in corner.Value)
                        {
                            Rect rect = sprite.textureRect;
                            Color c = sprite.texture.GetPixel(x+(int)rect.x, y+(int)rect.y);
                            if(c.a != 0) texture.SetPixel(x, y, c);
                        }
                    }
                }

                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + "/Resources/Sprites/tile_transition_water_"+i+".png", bytes);

                i++;
            }
        }

        // AssetDatabase.CreateAsset(ruleTile, "Assets/Resources/Tiles/Ground/water.png.asset");

        AssetDatabase.Refresh();
    }
}

#endif