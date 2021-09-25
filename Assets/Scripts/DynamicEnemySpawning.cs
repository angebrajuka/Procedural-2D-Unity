using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [HideInInspector] public static DynamicEnemySpawning instance;

    // hierarchy
    public GameObject enemyPrefab;
    public GameObject bloodSplatter;
    public float minRadius, maxRadius;

    // stats
    public static Dictionary<string, Enemy> enemies;
    public static LinkedList<EnemyObject> enemyObjects = new LinkedList<EnemyObject>();
    public static int totalDifficulty=0;
    public static float timer;
    readonly string[] states = {"_run", "_melee", "_projectile"};

    public void Init()
    {
        instance = this;
        totalDifficulty = 0;
        enemies = new Dictionary<string, Enemy>();
        var enemies_ = JsonUtility.FromJson<EnemiesJson>(Resources.Load<TextAsset>("EnemyData/enemies").text).enemies;
        foreach(var enemy in enemies_)
        {
            enemy.sprites = new Sprite[3][];
            for(int s=0; s<3; s++)
            {
                enemy.sprites[s] = Resources.LoadAll<Sprite>("Sprites/Enemies/"+enemy.name+states[s]);
            }
            enemies.Add(enemy.name, enemy);
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
        return PlayerStats.difficulty * 30 + 20;
    }

    public static float GetDynamicDifficulty()
    {
        return (float)totalDifficulty / GetDifficultyValue();
    }

    public EnemyObject Spawn(string name, bool autoPosition=true, Vector2 position=default(Vector2))
    {
        var enemy = enemies[name];
        if(autoPosition)
        {
            position = Random.insideUnitCircle.normalized*Random.Range(minRadius, maxRadius) + PlayerMovement.rb.position;
        }
        GameObject gameObject = Instantiate(enemyPrefab, position, Quaternion.identity, Entities.t);
        EnemyObject enemyObject = gameObject.GetComponent<EnemyObject>();
        enemyObject.enemy = enemy;
        enemyObject.awake = true;
        enemyObjects.AddLast(enemyObject);
        totalDifficulty += enemy.difficulty;

        return enemyObject;
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

    public static bool SpawnEnemies()
    {
        return (DaylightCycle.time > (DaylightCycle.k_EVENING + DaylightCycle.k_NIGHT) / 2 || DaylightCycle.time < DaylightCycle.k_DAY / 2);
    }

    void Update()
    {
        if(!PauseHandler.paused && SpawnEnemies())
        {
            timer -= Time.deltaTime;
            if(timer <= 0 && totalDifficulty < GetDifficultyValue())
            {
                timer = UnityEngine.Random.value*0+0.5f;
                Spawn("spider");
            }
        }
    }
}
