#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using static UnityEngine.Vector3Int;
using System.IO;
using System.Linq;

public class _EditorWaterGen : MonoBehaviour
{
    public Sprite[] water;
    
    public Sprite L, U, R, D, LU, UR, RD, DL, LR, UD, URD, RDL, DLU, LUR, LURD; // all edge combos
    public Sprite ul, ur, bl, br; // all corners


    bool Intersects(Vector3Int[] first, Vector3Int[] second)
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

    Vector3Int[] EdgeToCorners(Vector3Int edge)
    {
        if(edge.x != 0)
        {
            return new Vector3Int[]
            {
                new Vector3Int(edge.x, 1, 0),
                new Vector3Int(edge.x, -1, 0)
            };
        }
        else if(edge.y != 0)
        {
            return new Vector3Int[]
            {
                new Vector3Int(1, edge.y, 0),
                new Vector3Int(-1, edge.y, 0)
            };
        }
        return new Vector3Int[]{};
    }

    static SpriteMetaData SpriteMeta(int alignment, Vector4 border, string name, Vector2 pivot, Rect rect)
    {
        var data = new SpriteMetaData();
        data.alignment = alignment;
        data.border = border;
        data.name = name;
        data.pivot = pivot;
        data.rect = rect;
        return data;
    }

    static Sprite[] SaveSpriteAsAsset(int i)
    {
        var proj_path = "Assets/Resources/Sprites/Tiles/Water/"+"water_"+i+".png";
    
        AssetDatabase.Refresh();
    
        var ti = AssetImporter.GetAtPath(proj_path) as TextureImporter;
        ti.spritePixelsPerUnit = 16;
        ti.mipmapEnabled = false;
        ti.textureType = TextureImporterType.Sprite;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.spriteImportMode = SpriteImportMode.Multiple;
        ti.isReadable = true;
        ti.filterMode = FilterMode.Point;
        ti.spritesheet = new SpriteMetaData[]
        {
            SpriteMeta(9, ti.spriteBorder,  "water_0", new Vector2(0.5f, 0.5f), new Rect(0, 0, 16, 16)),
            SpriteMeta(9, ti.spriteBorder, "water_1", new Vector2(0.5f, 0.5f), new Rect(16, 0, 16, 16)),
        };

        Debug.Log(ti.spritesheet[0].pivot);
    
        EditorUtility.SetDirty(ti);
        ti.SaveAndReimport();

        AssetDatabase.Refresh();

        return Resources.LoadAll<Sprite>("Sprites/Tiles/Water/"+"water_"+i);
    }

    public void Generate()
    {
        var edges = new Dictionary<Vector3Int[], Sprite>()
        {
            {new Vector3Int[] { },                          null},
            {new Vector3Int[] { left },                     L},
            {new Vector3Int[] { up },                       U},
            {new Vector3Int[] { right },                    R},
            {new Vector3Int[] { down },                     D},
            {new Vector3Int[] { left, up },                 LU},
            {new Vector3Int[] { up, right },                UR},
            {new Vector3Int[] { right, down },              RD},
            {new Vector3Int[] { down, left },               DL},
            {new Vector3Int[] { left, right },              LR},
            {new Vector3Int[] { up, down },                 UD},
            {new Vector3Int[] { up, right, down },          URD},
            {new Vector3Int[] { right, down, left },        RDL},
            {new Vector3Int[] { down, left, up },           DLU},
            {new Vector3Int[] { left, up, right },          LUR},
            {new Vector3Int[] { left, up, right, down},     LURD},
        };

        var corners = new Dictionary<Vector3Int[], Sprite[]>()
        {
            {new Vector3Int[] { left, up, right, down },    new Sprite[]{ul, ur, br, bl}},
            {new Vector3Int[] { left, up, right, down },    new Sprite[]{ul, ur, br}},
            {new Vector3Int[] { left, up, right, down },    new Sprite[]{ur, br, bl}},
            {new Vector3Int[] { left, up, right, down },    new Sprite[]{br, bl, ul}},
            {new Vector3Int[] { left, up, right, down },    new Sprite[]{bl, ul, ur}},
            {new Vector3Int[] { left, up, right },          new Sprite[]{ul, ur}},
            {new Vector3Int[] { up, right, down },          new Sprite[]{ur, br}},
            {new Vector3Int[] { right, down, left },        new Sprite[]{br, bl}},
            {new Vector3Int[] { down, left, up },           new Sprite[]{bl, ul}},
            {new Vector3Int[] { left, up, right, down },    new Sprite[]{ul, br}},
            {new Vector3Int[] { left, up, right, down },    new Sprite[]{ur, bl}},
            {new Vector3Int[] { left, up },                 new Sprite[]{ul}},
            {new Vector3Int[] { up, right },                new Sprite[]{ur}},
            {new Vector3Int[] { right, down },              new Sprite[]{br}},
            {new Vector3Int[] { down, left },               new Sprite[]{bl}},
            {new Vector3Int[] { },                          new Sprite[]{ }},
        };

        var cornerRuleVecs = new Vector3Int[][]
        {
            new Vector3Int[]{ up+left, up+right, down+right, down+left},
            new Vector3Int[]{ up+left, up+right, down+right },
            new Vector3Int[]{ up+right, down+right, down+left },
            new Vector3Int[]{ down+right, down+left, up+left },
            new Vector3Int[]{ down+left, up+left, up+right},
            new Vector3Int[]{ left+up, up+right },
            new Vector3Int[]{ up+right, down+right },
            new Vector3Int[]{ down+right, down+left },
            new Vector3Int[]{ down+left, up+left },
            new Vector3Int[]{ up+left, down+right },
            new Vector3Int[]{ up+right, down+left },
            new Vector3Int[]{ left+up },
            new Vector3Int[]{ up+right },
            new Vector3Int[]{ right+down },
            new Vector3Int[]{ down+left },
            new Vector3Int[]{ },
        };

        AdvancedRuleTile ruleTile = ScriptableObject.CreateInstance("AdvancedRuleTile") as AdvancedRuleTile;
        ruleTile.m_DefaultSprite = water[0];
        ruleTile.m_DefaultColliderType = Tile.ColliderType.None;

        RuleTile.TilingRule defaultRule = new RuleTile.TilingRule();
        var dictAll = new Dictionary<Vector3Int, int>()
        {
            {new Vector3Int(-1, -1, 0), AdvancedRuleTile.Neighbor.This},
            {new Vector3Int(0, -1, 0),  AdvancedRuleTile.Neighbor.This},
            {new Vector3Int(1, -1, 0),  AdvancedRuleTile.Neighbor.This},
            {new Vector3Int(-1, 0, 0),  AdvancedRuleTile.Neighbor.This},
            {new Vector3Int(1, 0, 0),   AdvancedRuleTile.Neighbor.This},
            {new Vector3Int(-1, 1, 0),  AdvancedRuleTile.Neighbor.This},
            {new Vector3Int(0, 1, 0),   AdvancedRuleTile.Neighbor.This},
            {new Vector3Int(1, 1, 0),   AdvancedRuleTile.Neighbor.This},
        };
        var allSides = new Vector3Int[] { left, up, right, down };
        defaultRule.ApplyNeighbors(dictAll);
        defaultRule.m_Sprites = new Sprite[]{water[0], water[1]};
        defaultRule.m_ColliderType = Tile.ColliderType.None;
        defaultRule.m_Output = RuleTile.TilingRule.OutputSprite.Animation;
        defaultRule.m_MinAnimationSpeed = 2;
        defaultRule.m_MaxAnimationSpeed = 3;
        ruleTile.m_TilingRules.Add(defaultRule);

        int i=0;
        foreach(var edge in edges)
        {
            int cornerIndex = 0;
            foreach(var corner in corners)
            {
                if(Intersects(edge.Key, corner.Key) || (edge.Key.Length == 0 && corner.Key.Length == 0))
                {
                    cornerIndex ++;
                    continue;
                }

                var texture = new Texture2D(32, 16);

                var pixels = water[0].texture.GetPixels(0, 0, 16, 16);
                texture.SetPixels(0, 0, 16, 16, pixels);
                pixels = water[1].texture.GetPixels(16, 0, 16, 16);
                texture.SetPixels(16, 0, 16, 16, pixels);

                Texture2D transition = new Texture2D(16, 16);
                for(int x=0; x<16; x++)
                {
                    for(int y=0; y<16; y++)
                    {
                        transition.SetPixel(x, y, Color.clear);

                        if(edge.Value != null)
                        {
                            Rect rect = edge.Value.textureRect;
                            Color c = edge.Value.texture.GetPixel(x+(int)rect.x, y+(int)rect.y);
                            if(c.a != 0) transition.SetPixel(x, y, c);
                        }

                        foreach(var sprite in corner.Value)
                        {
                            Rect rect = sprite.textureRect;
                            Color c = sprite.texture.GetPixel(x+(int)rect.x, y+(int)rect.y);
                            if(c.a != 0) transition.SetPixel(x, y, c);
                        }
                    }
                }
                for(int x=0; x<32; x++)
                {
                    for(int y=0; y<16; y++)
                    {
                        Color c = transition.GetPixel(x%16, y);
                        if(c.a != 0) texture.SetPixel(x, y, c);
                    }
                }
                
                texture.Apply();

                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + "/Resources/Sprites/Tiles/Water/water_"+i+".png", bytes);

                Sprite sprite0 = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
                Sprite sprite1 = Sprite.Create(texture, new Rect(16, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);

                Sprite[] sprites = SaveSpriteAsAsset(i);
                
                RuleTile.TilingRule rule = new RuleTile.TilingRule();
                rule.m_Sprites = sprites;
                var dict = new Dictionary<Vector3Int, int>();
                foreach(var vec in edge.Key)
                {
                    dict.Add(vec, AdvancedRuleTile.Neighbor.NotThis);
                    // foreach(var veci in EdgeToCorners(vec))
                    // {
                    //     if(!dict.ContainsKey(veci)) dict.Add(veci, AdvancedRuleTile.Neighbor.NotThis);
                    // }
                }
                foreach(var vec in cornerRuleVecs[cornerIndex])
                {
                    dict.Add(vec, AdvancedRuleTile.Neighbor.NotThis);
                }
                
                foreach(var vec in allSides)
                {
                    if(!Intersects(new Vector3Int[]{vec}, dict.Keys.ToArray()))
                    {
                        dict.Add(vec, AdvancedRuleTile.Neighbor.This);
                    }
                }

                rule.ApplyNeighbors(dict);
                rule.m_ColliderType = Tile.ColliderType.Grid;
                rule.m_Output = RuleTile.TilingRule.OutputSprite.Animation;
                rule.m_MinAnimationSpeed = 2;
                rule.m_MaxAnimationSpeed = 3;

                ruleTile.m_TilingRules.Add(rule);

                i++;
                cornerIndex ++;
            }
        }

        RuleTile.TilingRule transparentRule = new RuleTile.TilingRule();
        transparentRule.m_Sprites = new Sprite[]{null};
        transparentRule.m_ColliderType = Tile.ColliderType.None;

        ruleTile.m_TilingRules.Add(transparentRule);
        AssetDatabase.CreateAsset(ruleTile, "Assets/Resources/Tiles/Ground/water.asset");
        AssetDatabase.Refresh();
    }
}

#endif