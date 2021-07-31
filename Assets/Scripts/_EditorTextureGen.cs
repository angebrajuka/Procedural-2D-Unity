#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class _EditorTextureGen : MonoBehaviour
{
    public Texture2D map;
    public Texture2D[] textures;
    public string newTextureName;
    public int tileWidth;

    static readonly byte[][] pattern = new byte[][]
    {
        new byte[]{0},
        new byte[]{1},
        new byte[]{2},
        new byte[]{3},
        new byte[]{0, 1},
        new byte[]{0, 2},
        new byte[]{0, 3},
        new byte[]{1, 2},
        new byte[]{1, 3},
        new byte[]{2, 3},
        new byte[]{0, 1, 2},
        new byte[]{1, 2, 3},
        new byte[]{2, 3, 0},
        new byte[]{3, 0, 1},
        new byte[]{0, 1, 2, 3}
    };

    public void GenerateTexture()
    {
        Texture2D newTexture = new Texture2D(tileWidth, tileWidth*15);

        // for(int i=0; i<15; i++)
        // {
        //     // Texture2D miniMap = new Texture2D(tileWidth, tileWidth);
            
        //     for(int y=0; y<tileWidth; y++)
        //     {
        //         for(int x=0; x<tileWidth; x++)
        //         {
        //             // for(int j=0; j<pattern[i].Length; j++)
        //             // {

        //             // }

        //         }
        //     }
        // }

        for(int i=0; i<15; i++)
        {
            for(int y=0; y<tileWidth; y++)
            {
                for(int x=0; x<tileWidth; x++)
                {
                    int bw = 1;
                    for(int p=0; p<pattern[i].Length; p++)
                    {
                        if(Mathf.Round(map.GetPixel(x, y+tileWidth*pattern[i][p]).r) == 0)
                        {
                            bw = 0;
                        }
                    }
                    newTexture.SetPixel(x, y+i*tileWidth, textures[bw].GetPixel(x, y));
                }
            }
        }

        byte[] bytes = newTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Resources/Sprites/" + newTextureName + ".png", bytes);
        AssetDatabase.Refresh();
    }
}

#endif