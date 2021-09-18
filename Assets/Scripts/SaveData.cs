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
        item = gridItem.item.name;
        x = gridItem.GetX();
        y = gridItem.GetY();
        rotated = gridItem.rotated;
        count = gridItem.count;
        ammo = gridItem.ammo;
        equipped = (PlayerStats.currentItemNode == node);
    }
}

[System.Serializable]
public class EnemySerializable
{
    public string name;
    public float health;
    public float[] position;
    public float[] velocity;

    public EnemySerializable(EnemyObject enemy)
    {
        name = enemy.enemy.name;
        health = enemy.m_target.health;
        position = new float[]{enemy.m_rigidbody.position.x, enemy.m_rigidbody.position.y};
        velocity = new float[]{enemy.m_rigidbody.velocity.x, enemy.m_rigidbody.velocity.y};
    }
}

[System.Serializable]
public class SaveData
{
    byte difficulty;
    float seed;
    GridItemSerializable[] inventoryItems;
    EnemySerializable[] enemies;
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

        enemies = new EnemySerializable[DynamicEnemySpawning.enemyObjects.Count];
        int e=0;
        foreach(var enemy in DynamicEnemySpawning.enemyObjects)
        {
            enemies[e++] = new EnemySerializable(enemy);
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

        DynamicEnemySpawning.Reset();
        foreach(var enemy in enemies)
        {
            var enemyObject = DynamicEnemySpawning.instance.Spawn(enemy.name, false, new Vector2(enemy.position[0], enemy.position[1]));
            enemyObject.Start();
            enemyObject.m_target.health = enemy.health;
            enemyObject.m_rigidbody.velocity = new Vector2(enemy.velocity[0], enemy.velocity[1]);
        }

        PlayerStats.target.health = health;
        PlayerStats.energy = energy;

        DaylightCycle.time = timeOfDay;

        PlayerStats.rigidbody.position = new Vector2(position[0], position[1]);
        PlayerStats.rigidbody.transform.position = PlayerStats.rigidbody.position;
    }
}
