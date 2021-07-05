using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    byte difficulty;
    GridItemSerializable[] inventoryItems;
    float health, energy;
    float[] position;

    public SaveData()
    {
        difficulty = PlayerStats.difficulty;
        
        inventoryItems = new GridItemSerializable[PlayerStats.inventory.items.Count];
        LinkedListNode<GridItem> node = PlayerStats.inventory.items.First;
        for(int i=0; i<inventoryItems.Length; i++)
        {
            inventoryItems[i] = new GridItemSerializable(node.Value);
            node = node.Next;
        }

        health = PlayerStats.target.health;
        energy = PlayerStats.energy;

        position = new float[2];
        position[0] = PlayerStats.rigidbody.position.x;
        position[1] = PlayerStats.rigidbody.position.y;
    }

    public void Load()
    {
        PlayerStats.difficulty = difficulty;

        PlayerStats.inventory.Clear();
        for(int i=0; i<inventoryItems.Length; i++)
        {
            PlayerStats.inventory.Add((Item)inventoryItems[i].item, inventoryItems[i].x, inventoryItems[i].y);
            if(inventoryItems[i].rotated) PlayerStats.inventory.items.Last.Value.Rotate();
            PlayerStats.inventory.items.Last.Value.count = inventoryItems[i].count;
            PlayerStats.inventory.items.Last.Value.UpdateCount();
        }

        PlayerStats.target.health = health;
        PlayerStats.energy = energy;

        PlayerStats.rigidbody.MovePosition(new Vector2(position[0], position[1]));
    }
}
