using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour {
    
    // components
    float sink;
    Material m_material;
    Rigidbody2D m_rigidbody;
    Collider2D m_collider;


    void Start() {
        m_material = transform.parent.GetComponent<SpriteRenderer>().material;
        m_rigidbody = transform.parent.GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        sink = 1;
    }

    void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer == 12 && m_rigidbody.velocity.magnitude < 2) {
            sink -= Time.deltaTime;
            if(sink <= 0) {
                Destroy(this);
                return;
            }
            m_material.SetFloat("_Sink", sink);
        }
    }
}
