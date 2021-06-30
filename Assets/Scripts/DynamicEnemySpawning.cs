using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public struct Enemy
{
    public static Enemy[] enemies = new Enemy[]
    {
        new Enemy(5, Enemy_Spider.OnStart, Enemy_Spider.OnOnDamage, Enemy_Spider.OnOnKill, Enemy_Spider.CalcPath)
    };

    public int difficulty;
    public Func<EnemyObject, bool> OnStart;
    public Func<EnemyObject, bool> OnOnDamage;
    public Func<EnemyObject, bool> OnOnKill;
    public Func<EnemyObject, bool> CalcPath;

    public Enemy(int difficulty, Func<EnemyObject, bool> OnStart, Func<EnemyObject, bool> OnOnDamage, Func<EnemyObject, bool> OnOnKill, Func<EnemyObject, bool> CalcPath)
    {
        this.difficulty = difficulty;
        this.OnStart = OnStart;
        this.OnOnDamage = OnOnDamage;
        this.OnOnKill = OnOnKill;
        this.CalcPath = CalcPath;
    }
}

public enum EnemyType
{
    SPIDER
}

public class DynamicEnemySpawning : MonoBehaviour
{
    // hierarchy
    public GameObject enemyPrefab;

    // stats
    public LinkedList<EnemyObject> enemyObjects;
    public int totalDifficulty;

    void Start()
    {
        enemyObjects = new LinkedList<EnemyObject>();
        totalDifficulty = 0;
    }

    public void Spawn(EnemyType type) {
        Enemy enemy = Enemy.enemies[(int)type];
        GameObject gameObject = Instantiate(enemyPrefab, PlayerStats.rigidbody.position, Quaternion.identity);
        EnemyObject enemyObject = gameObject.GetComponent<EnemyObject>();
        enemyObjects.AddLast(enemyObject);
        totalDifficulty += enemy.difficulty;
    }

    public void DeSpawn(EnemyObject enemyObject) {
        Enemy enemy = Enemy.enemies[(int)enemyObject.type];

        totalDifficulty -= enemy.difficulty;
    }

    void Update()
    {
        if(DaylightCycle.time > 140 || DaylightCycle.time < 5)
        {
            if(enemyObjects.Count < 1)
            {
                Spawn(EnemyType.SPIDER);
            }
        }
    }
}
