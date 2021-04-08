using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    // components
    Rigidbody2D m_rigidbody;
    Collider2D other;

    void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();
        other = null;
    }

    void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.layer == 8) {
            this.other = other;
        }
    }

    void Update() {
        if(other != null) {
            Vector2 dir = other.attachedRigidbody.position-m_rigidbody.position;
            dir.Normalize();
            m_rigidbody.AddForce(dir*50);
            if(Math.Closer(m_rigidbody.position, other.attachedRigidbody.position, 0.05f)) Destroy(gameObject);
        }
    }
}
