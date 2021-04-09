using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spider : Target {
    

    // hierarchy
    public EightDirectionAnimator m_animator;
    public Transform prefab_coinExplosion;
    

    // components
    [HideInInspector] public Rigidbody2D m_rigidbody;
    [HideInInspector] public Material m_material;


    // members
    Vector2 dir;
    float timer;
    bool following;
    float animationTimer;
    float flash;


    void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_material = GetComponent<SpriteRenderer>().material;
        timer = 0;
        animationTimer = 0;
        flash = 0;
    }


    public override void OnDamage(float damage) {
        Color c = m_animator.m_spriteRenderer.color;
        flash = 1;
        m_animator.m_spriteRenderer.color = c;
    }

    public override void OnKill(float damage) {
        Transform coinExplosion = Instantiate(prefab_coinExplosion);
        coinExplosion.position = transform.position;
        Destroy(this.gameObject);
    }

    public override void OnHeal(float heal) {}


    void NewTarget() {
        following = (Random.value > 0.4f);
        
        dir = following ? PlayerInput.rigidbody.position-m_rigidbody.position : (Math.vectors[(int)Mathf.Floor(Random.value*8)]);

        m_animator.direction = Math.AngleToDir8(Math.NormalizedVecToAngle(dir));
        if(m_animator.direction >= 8) m_animator.direction = 0;
        dir = Math.vectors[m_animator.direction];
        
        timer = Random.value+0.2f;
    }

    void OnCollisionStay2D(Collision2D other) {
        if(timer < 0.1f) {
            dir *= -1;
            m_animator.direction = Math.AngleToDir8(Math.NormalizedVecToAngle(dir));
            timer = 0.2f;
        }
    }

    void Update() {
        if(timer <= 0) {
            NewTarget();
        }

        animationTimer += Time.deltaTime;
        if(animationTimer > 0.05f) {
            animationTimer = 0;
            m_animator.state ++;
            if(m_animator.state > 3) m_animator.state = 0;
        }
        
        timer -= Time.deltaTime;

        if(flash > 0) {
            flash -= Time.deltaTime*8;
            m_material.SetFloat("_Blend", flash);
        }
    }

    void FixedUpdate() {
        m_rigidbody.velocity *= 0;
        m_rigidbody.AddForce(dir*120);
    }
}
