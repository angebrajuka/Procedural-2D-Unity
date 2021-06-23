using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour {
    
    void Start() {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.startLifetime.constantMax);
    }
}
