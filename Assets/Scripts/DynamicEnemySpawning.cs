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
    public static GameObject s_enemyPrefab;

    // stats
    public static LinkedList<EnemyObject> enemyObjects = new LinkedList<EnemyObject>();
    public static int totalDifficulty=0;
    public static float timer;
    public const float minRadius=30, maxRadius=45;

    public void Init()
    {
        s_enemyPrefab = enemyPrefab;
        totalDifficulty = 0;
    }

    public static void Reset()
    {
        while(enemyObjects.Count > 0)
        {
            DeSpawn(enemyObjects.First.Value);
        }
    }

    public static int GetDifficultyValue()
    {
        return PlayerStats.difficulty * 40 + 30;
    }

    public static float GetDynamicDifficulty()
    {
        return (float)totalDifficulty / GetDifficultyValue();
    }

    public static void Spawn(EnemyType type, bool autoPosition=true, Vector2 position=default(Vector2))
    {
        if(autoPosition)
        {
            position = new Vector2(0, 0);
            float radius = UnityEngine.Random.value * (maxRadius-minRadius) + minRadius;
            position.x = (UnityEngine.Random.value-0.5f) * radius * 2;
            position.y = Mathf.Sqrt(radius*radius - position.x*position.x) * (UnityEngine.Random.value > 0.5f ? 1 : -1);
            position += PlayerStats.rigidbody.position;
        }
        Enemy enemy = Enemy.enemies[(int)type];
        GameObject gameObject = Instantiate(s_enemyPrefab, position, Quaternion.identity);
        EnemyObject enemyObject = gameObject.GetComponent<EnemyObject>();
        enemyObjects.AddLast(enemyObject);
        totalDifficulty += enemy.difficulty;
    }

    public static void DeSpawn(EnemyObject enemyObject)
    {
        OnKilled(enemyObject);
        Destroy(enemyObject.gameObject);
    }

    public static void OnKilled(EnemyObject enemyObject)
    {
        Enemy enemy = Enemy.enemies[(int)enemyObject.type];

        totalDifficulty -= enemy.difficulty;

        enemyObjects.Remove(enemyObject);
    }

    void Update()
    {
        if(DaylightCycle.time > (DaylightCycle.k_EVENING + DaylightCycle.k_NIGHT) / 2 || DaylightCycle.time < DaylightCycle.k_DAY / 2)
        {
            timer -= Time.deltaTime;

            if(totalDifficulty < GetDifficultyValue() && timer <= 0)
            {
                timer = UnityEngine.Random.value*0+0.5f;

                Spawn(EnemyType.SPIDER);
            }
        }
    }
}
