using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridItemSerializable
{
    public byte item;
    public int x, y;
    public bool rotated;

    public GridItemSerializable(GridItem gridItem)
    {
        item = (byte)gridItem.item;
        x = gridItem.GetX();
        y = gridItem.GetY();
        rotated = gridItem.rotated;
    }
}
