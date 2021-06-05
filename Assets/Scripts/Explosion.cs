using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    private CircleCollider2D trigger;

    void Start() {
        trigger = GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        print(collider);
    }

    void OnEnable() {
        Destroy(gameObject, 10);
    }

    void Update() {
        trigger.radius += Time.deltaTime;
    }
}
