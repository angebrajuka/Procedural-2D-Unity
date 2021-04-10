using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade_Thrown : MonoBehaviour {
    
    Rigidbody2D m_rigidbody;

    void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void OnCollisionEnter2D(Collision2D collision) {
        if(m_rigidbody.velocity.magnitude < 4) return;
        Target target = collision.transform.GetComponent<Target>();
        if(target != null) {
            target.Damage(15);
        }
    }
}
