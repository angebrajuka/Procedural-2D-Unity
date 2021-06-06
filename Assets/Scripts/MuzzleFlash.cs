using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class MuzzleFlash : MonoBehaviour {

    UnityEngine.Experimental.Rendering.Universal.Light2D light2D;

    void Start() {
        light2D = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        Destroy(gameObject, 0.05f);
        for(int i=0; i<light2D.shapePath.Length; i++) {
            light2D.shapePath[i] += Vector3.right*(Random.value-0.5f)*0.3f;
            light2D.shapePath[i] += Vector3.forward*(Random.value-0.5f)*0.3f;
        }
    }

    void FixedUpdate() {
        if(light2D.intensity > 0) {
            light2D.intensity -= 0.2f;
            if(light2D.intensity < 0) light2D.intensity = 0;
        }
    }
}
