using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameState;
using static Singles;

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
        x = gridItem.X(Inventory.instance.grids[0]);
        y = gridItem.Y(Inventory.instance.grids[0]);
        rotated = gridItem.rotated;
        count = gridItem.count;
        ammo = gridItem.ammo;
        equipped = (PlayerState.currentItemNode == node);
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
    ushort seed;
    GridItemSerializable[] inventoryItems;
    EnemySerializable[] enemies;
    
    float health, energy;
    float[] position;
    float timeOfDay;

    public SaveData()
    {
        difficulty = gameState.difficulty;
        seed = WorldGen.seed;
        
        Inventory.instance.Close();
        inventoryItems = new GridItemSerializable[Inventory.instance.items.Count];
        LinkedListNode<GridItem> node = Inventory.instance.items.First;
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

        health = PlayerTarget.target.health;
        energy = PlayerStats.energy;

        position = new float[2];
        position[0] = PlayerMovement.rb.position.x;
        position[1] = PlayerMovement.rb.position.y;

        timeOfDay = DaylightCycle.time;
    }

    public void Load()
    {
        FadeTransition.black = true;
        MenuHandler.CloseAll();

        gameState.difficulty = difficulty;
        WorldGen.SetSeed(seed);
        singles.worldGen.GenerateMap();

        Inventory.instance.Clear();
        for(int i=0; i<inventoryItems.Length; i++)
        {
            GridItem gridItem = Inventory.instance.Add(inventoryItems[i].item, inventoryItems[i].x, inventoryItems[i].y);
            if(inventoryItems[i].rotated) gridItem.Rotate();
            gridItem.count = inventoryItems[i].count;
            gridItem.ammo = inventoryItems[i].ammo;
            gridItem.UpdateCount();

            if(inventoryItems[i].equipped)
            {
                Inventory.instance.Equip(Inventory.instance.items.Last);
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

        PlayerTarget.target.health = health;
        PlayerStats.energy = energy;

        DaylightCycle.time = timeOfDay;

        PlayerMovement.rb.position = new Vector2(position[0], position[1]);
        PlayerMovement.rb.transform.position = PlayerMovement.rb.position;
    }
}
