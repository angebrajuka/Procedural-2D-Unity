﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spider : MonoBehaviour {
    

    // hierarchy
    public EightDirectionAnimator m_animator;
    public Transform prefab_coinExplosion;
    public bool awake;
    

    // components
    [HideInInspector] public Rigidbody2D m_rigidbody;
    [HideInInspector] public Material m_material;
    [HideInInspector] public Target m_target;


    // members
    Vector2 dir;
    float timer;
    bool following;
    float animationTimer;
    float flash;


    void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_material = GetComponent<SpriteRenderer>().material;
        m_target = GetComponent<Target>();
        m_target.OnDamage = OnDamage;
        m_target.OnKill = OnKill;
        timer = 0;
        animationTimer = 0;
        flash = 0;
    }

    public bool OnDamage(float damage) {
        Color c = m_animator.m_spriteRenderer.color;
        flash = 1;
        m_animator.m_spriteRenderer.color = c;
        return true;
    }

    public bool OnKill(float damage) {
        Transform coinExplosion = Instantiate(prefab_coinExplosion);
        coinExplosion.position = transform.position;
        Destroy(this.gameObject);
        return true;
    }


    void NewTarget(bool forceWander=false) {
        following = !forceWander && (Random.value > 0.4f);
        
        dir = following ? PlayerInput.m_rigidbody.position-m_rigidbody.position : (Math.vectors[(int)Mathf.Floor(Random.value*8)]);

        m_animator.direction = Math.AngleToDir8(Math.NormalizedVecToAngle(dir));
        if(m_animator.direction >= 8) m_animator.direction = 0;
        dir = Math.vectors[m_animator.direction];
        
        timer = Random.value+0.2f;
    }

    void OnCollisionStay2D(Collision2D other) {
        if(timer < 0.1f) {
            // dir *= -1;
            // m_animator.direction = Math.AngleToDir8(Math.NormalizedVecToAngle(dir));
            // timer = 0.2f;
            NewTarget(true);
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
        if(awake) {
            m_rigidbody.velocity *= 0;
            m_rigidbody.AddForce(dir*320);
        }
    }
}
