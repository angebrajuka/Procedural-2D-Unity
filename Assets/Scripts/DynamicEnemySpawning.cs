using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEnemySpawning : MonoBehaviour
{
    public GameObject h_enemyPrefab;

    public static GameObject enemyPrefab;
    public static LinkedList<Enemy> enemies = new LinkedList<Enemy>();
    public static int totalDifficulty = 0;

    void Start()
    {
        enemyPrefab = h_enemyPrefab;
    }

    public static void Spawn(EnemyType type) {
        EnemyFunctions enemy = Enemy.enemyFunctions[(int)type];

        totalDifficulty += enemy.difficulty;
    }

    public static void DeSpawn(Enemy enemyObject) {
        EnemyFunctions enemy = Enemy.enemyFunctions[(int)enemyObject.type];

        totalDifficulty -= enemy.difficulty;
    }

    void Update()
    {
        if(DaylightCycle.time > 140 || DaylightCycle.time < 5)
        {

        }
    }
}
