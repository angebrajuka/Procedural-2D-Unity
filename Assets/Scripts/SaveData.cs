using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    byte difficulty;
    float seed;
    GridItemSerializable[] inventoryItems;
    float health, energy;
    float[] position;
    float timeOfDay;

    public SaveData()
    {
        difficulty = PlayerStats.difficulty;
        seed = ProceduralGeneration.seed_main;
        
        inventoryItems = new GridItemSerializable[PlayerStats.inventory.items.Count];
        LinkedListNode<GridItem> node = PlayerStats.inventory.items.First;
        for(int i=0; i<inventoryItems.Length; i++)
        {
            inventoryItems[i] = new GridItemSerializable(node);
            node = node.Next;
        }

        health = PlayerStats.target.health;
        energy = PlayerStats.energy;

        position = new float[2];
        position[0] = PlayerStats.rigidbody.position.x;
        position[1] = PlayerStats.rigidbody.position.y;

        timeOfDay = DaylightCycle.time;
    }

    public void Load()
    {
        PlayerStats.difficulty = difficulty;
        ProceduralGeneration.SetSeed(seed);

        PlayerStats.inventory.Clear();
        for(int i=0; i<inventoryItems.Length; i++)
        {
            GridItem gridItem = PlayerStats.inventory.Add(inventoryItems[i].item, inventoryItems[i].x, inventoryItems[i].y);
            if(inventoryItems[i].rotated) gridItem.Rotate();
            gridItem.count = inventoryItems[i].count;
            gridItem.ammo = inventoryItems[i].ammo;
            gridItem.UpdateCount();

            if(inventoryItems[i].equipped)
            {
                PlayerStats.inventory.Equip(PlayerStats.inventory.items.Last);
            }
        }

        PlayerStats.target.health = health;
        PlayerStats.energy = energy;

        DaylightCycle.time = timeOfDay;

        PlayerStats.rigidbody.position = new Vector2(position[0], position[1]);
    }
}
