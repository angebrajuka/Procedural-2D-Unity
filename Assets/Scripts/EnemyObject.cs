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


    // members
    [HideInInspector] public Vector2 targetMovement;
    bool following;
    float flash;
    public int state;
    public float frame;
    [HideInInspector] public bool awake;
    [HideInInspector] public Enemy enemy;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        m_material = m_renderer.material;
        m_target = GetComponent<Target>();
        m_target.OnDamage = OnDamage;
        m_target.OnKill = OnKill;
        flash = 0;
        state = 0;
        frame = 0;
    }

    public bool OnDamage(float damage)
    {
        flash = 1;
        return true;
    }

    public bool OnKill(float damage)
    {
        DynamicEnemySpawning.OnKilled(this);
        Destroy(this.gameObject);
        return true;
    }

    void NewTarget(bool forceWander=false)
    {
        following = !forceWander && (UnityEngine.Random.value > 0.4f);
        
        targetMovement = following ? PlayerStats.rigidbody.position-m_rigidbody.position : Random.insideUnitCircle;
        targetMovement.Normalize();
        m_renderer.flipX = targetMovement.x < 0;
        
        frame = (UnityEngine.Random.value+0.2f);
    }

    // void OnCollisionStay2D(Collision2D other)
    // {
    //     // if(timer < 0.1f)
    //     // {
    //         NewTarget(true);
    //     // }
    // }

    void FixedUpdate()
    {
        if(awake)
        {
            if(state == 0 && Vector2.Distance(m_rigidbody.position, PlayerStats.rigidbody.position) <= enemy.range_melee)
            {
                state = 1;
                frame = 0;

                targetMovement *= 0;
            }

            frame += Time.fixedDeltaTime*enemy.anim_speeds[state];
            if(frame >= enemy.sprites[state].Length)
            {
                switch(state)
                {
                case 0:
                    NewTarget();
                    break;
                case 1:
                    PlayerStats.target.Damage(10);
                    state = 0;
                    break;
                case 2:
                    
                    break;
                default: break;
                }
                frame = 0;
            }

            m_renderer.sprite = enemy.sprites[state][(int)Mathf.Floor(frame)];
            

            if(flash > 0)
            {
                flash -= Time.fixedDeltaTime*6;
                if(flash < 0) flash = 0;
                m_material.SetFloat("_Blend", flash);
            }
            
            m_rigidbody.velocity *= 0;
            m_rigidbody.AddForce(targetMovement*enemy.speed_move);
        }
    }
}
