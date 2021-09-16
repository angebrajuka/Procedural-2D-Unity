using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridItemSerializable
{
    public string item;
    public int x, y;
    public bool rotated;
    public int count;
    public int ammo;
    public bool equipped;

    public GridItemSerializable(LinkedListNode<GridItem> node)
    {
        GridItem gridItem = node.Value;
        item = gridItem.item;
        x = gridItem.GetX();
        y = gridItem.GetY();
        rotated = gridItem.rotated;
        count = gridItem.count;
        ammo = gridItem.ammo;
        equipped = (PlayerStats.currentItemNode == node);
    }
}
