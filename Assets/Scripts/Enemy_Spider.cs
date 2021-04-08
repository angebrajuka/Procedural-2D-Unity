using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spider : Target {
    

    // hierarchy
    public EightDirectionAnimator m_animator;
    public Transform prefab_coinExplosion;
    

    // components
    [HideInInspector] public Rigidbody2D m_rigidbody;


    // members
    Vector2 dir;
    float timer;
    bool following;
    float animationTimer;


    void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();
        timer = 0;
        animationTimer = 0;
    }

    public override void OnDamage(float damage) {}
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

        // dir = Vector2.right;
        
        timer = Random.value+0.2f;
    }

    void OnCollisionEnter(Collision other) {
        NewTarget();
    }

    void OnCollisionStay(Collision other) {
        NewTarget();
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
        
        m_rigidbody.velocity *= 0;
        m_rigidbody.AddForce(dir*120);
    }
}
