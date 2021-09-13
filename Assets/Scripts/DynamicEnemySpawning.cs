﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class EnemiesJson
{
    public Enemy[] enemies;
}

[System.Serializable]
public class Enemy
{
    public string name;
    public int difficulty;
    public float range_melee, range_projectile;
    public float attack_cooldown_min;
    public float attack_cooldown_max;
    public float speed_move;
    public Sprite[][] sprites;
    public float[] anim_speeds;
}

public class DynamicEnemySpawning : MonoBehaviour
{
    // hierarchy
    public GameObject enemyPrefab;

    // stats
    public static Enemy[] enemies;
    public static LinkedList<EnemyObject> enemyObjects = new LinkedList<EnemyObject>();
    public static int totalDifficulty=0;
    public static float timer;
    public const float minRadius=30, maxRadius=45;
    readonly string[] states = {"_run", "_melee", "_projectile"};

    public void Init()
    {
        totalDifficulty = 0;
        enemies = JsonUtility.FromJson<EnemiesJson>(Resources.Load<TextAsset>("EnemyData/enemies").text).enemies;
        foreach(var enemy in enemies)
        {
            enemy.sprites = new Sprite[3][];
            for(int s=0; s<3; s++)
            {
                enemy.sprites[s] = Resources.LoadAll<Sprite>("Sprites/Enemies/"+enemy.name+states[s]);
            }
        }
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

    public void Spawn(Enemy enemy, bool autoPosition=true, Vector2 position=default(Vector2))
    {
        if(autoPosition)
        {
            position = new Vector2(0, 0);
            float radius = UnityEngine.Random.value * (maxRadius-minRadius) + minRadius;
            position.x = (UnityEngine.Random.value-0.5f) * radius * 2;
            position.y = Mathf.Sqrt(radius*radius - position.x*position.x) * (UnityEngine.Random.value > 0.5f ? 1 : -1);
            position += PlayerStats.rigidbody.position;
        }
        GameObject gameObject = Instantiate(enemyPrefab, position, Quaternion.identity);
        EnemyObject enemyObject = gameObject.GetComponent<EnemyObject>();
        enemyObject.enemy = enemy;
        enemyObject.awake = true;
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
        totalDifficulty -= enemyObject.enemy.difficulty;
        enemyObjects.Remove(enemyObject);
    }

    void Update()
    {
        if(!PauseHandler.paused && (DaylightCycle.time > (DaylightCycle.k_EVENING + DaylightCycle.k_NIGHT) / 2 || DaylightCycle.time < DaylightCycle.k_DAY / 2))
        {
            timer -= Time.deltaTime;
            if(timer <= 0 && totalDifficulty < GetDifficultyValue())
            {
                timer = UnityEngine.Random.value*0+0.5f;
                Spawn(enemies[0]);
            }
        }
    }
}
