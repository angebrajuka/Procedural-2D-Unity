using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : MonoBehaviour
{
    // components
    [HideInInspector] public Rigidbody2D m_rigidbody;
    [HideInInspector] public Material m_material;
    [HideInInspector] public Target m_target;
    [HideInInspector] public SpriteRenderer m_renderer;
    [HideInInspector] public CircleCollider2D m_trigger;


    // members
    [HideInInspector] public Dictionary<Transform, Vector2> nearbyColliders;
    [HideInInspector] public Vector2 targetMovement;
    public int state;
    public float frame;
    public float attackCooldown;
    [HideInInspector] public bool awake;
    [HideInInspector] public Enemy enemy;

    public void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        m_material = m_renderer.material;
        m_target = GetComponent<Target>();
        m_target.OnDamage = OnDamage;
        m_target.OnKill = OnKill;
        m_target.OnHeal = Target.DefaultHeal;
        m_trigger = transform.GetChild(1).GetComponent<CircleCollider2D>();
        nearbyColliders = new Dictionary<Transform, Vector2>();
        state = 0;
        frame = (UnityEngine.Random.value+0.2f);;
        attackCooldown = 0;
    }

    public bool OnDamage(float damage, float angle)
    {
        m_rigidbody.AddForce(Math.AngleToVector2(angle)*300*damage);
        Instantiate(DynamicEnemySpawning.instance.bloodSplatter, m_rigidbody.position, Quaternion.Euler(0, 0, angle));
        return true;
    }

    public bool OnKill(float damage, float angle)
    {
        DynamicEnemySpawning.OnKilled(this);
        Destroy(this.gameObject);
        return true;
    }

    void UpdateTarget()
    {
        targetMovement = (PlayerMovement.rb.position-m_rigidbody.position).normalized;
        if(!DynamicEnemySpawning.SpawnEnemies()) targetMovement *= -1;

        foreach(var pair in nearbyColliders)
        {
            targetMovement -= (pair.Value-m_rigidbody.position).normalized*3;
        }

        targetMovement.Normalize();

        m_renderer.flipX = targetMovement.x < 0;
    }

    bool Range(float range)
    {
        return Vector2.Distance(m_rigidbody.position, PlayerMovement.rb.position) <= range;
    }

    void Attack(byte state)
    {
        attackCooldown = Random.Range(enemy.attack_cooldown_min, enemy.attack_cooldown_max);
        this.state = state;
        frame = 0;
        targetMovement *= 0;
    }

    void FacePlayer()
    {
        m_renderer.flipX = PlayerMovement.rb.position.x < m_rigidbody.position.x;
    }

    void FixedUpdate()
    {
        if(Vector2.Distance(m_rigidbody.position, PlayerMovement.rb.position) >= DynamicEnemySpawning.instance.maxRadius*2)
        {
            DynamicEnemySpawning.DeSpawn(this);
        }

        if(!PauseHandler.paused && awake)
        {
            switch(state)
            {
            case 0:
                if(attackCooldown > 0)
                {
                    attackCooldown -= Time.fixedDeltaTime;
                }
                else
                {
                    if(Range(enemy.range_melee))
                    {
                        Attack(1);
                    }
                    else if(Range(enemy.range_projectile))
                    {
                        Attack(2);
                    }
                }
                break;
            case 1:
                FacePlayer();
                break;
            case 2:

                break;
            default: break;
            }
    
            frame += Time.fixedDeltaTime*enemy.anim_speeds[state];
            if(frame >= enemy.sprites[state].Length)
            {
                switch(state)
                {
                case 0:
                    UpdateTarget();
                    break;
                case 1:
                    if(Range(enemy.range_melee))
                    {
                        PlayerTarget.target.Damage(10);
                    }
                    state = 0;
                    UpdateTarget();
                    break;
                case 2:
                    
                    break;
                default: break;
                }
                frame = 0;
            }

            m_renderer.sprite = enemy.sprites[state][(int)Mathf.Floor(frame)];
            m_rigidbody.AddForce(targetMovement*enemy.speed_move);
        }
    }
}
