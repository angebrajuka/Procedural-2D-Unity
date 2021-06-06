using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Explosion : MonoBehaviour {

    private CircleCollider2D trigger;
    public UnityEngine.Experimental.Rendering.Universal.Light2D m_light;

    void Start() {
        trigger = GetComponent<CircleCollider2D>();
        Destroy(gameObject, 0.3f);
        Destroy(m_light.gameObject, 0.05f);
    }

    void OnTriggerStay2D(Collider2D collider) {
        Target target = collider.transform.GetComponent<Target>();
        if(target != null) {
            target.Damage(2);
            Rigidbody2D rb = collider.transform.GetComponent<Rigidbody2D>();
            if(rb != null) {
                Vector3 vec = rb.position;
                vec.x -= transform.position.x;
                vec.y -= transform.position.y;
                vec.Normalize();
                rb.AddForce(vec*200);
            }
        }
    }

    void OnEnable() {
        Destroy(gameObject, 10);
    }

    void FixedUpdate() {
        trigger.radius += 0.4f;
    }
}
