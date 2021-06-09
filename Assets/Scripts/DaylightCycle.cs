using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class DaylightCycle : MonoBehaviour {

    const float k_MORNING=240, k_DAY=15, k_EVENING=135, k_NIGHT=150;
    public float brightness_day, brightness_night;

    public static float time = k_DAY;

    UnityEngine.Experimental.Rendering.Universal.Light2D globalLight;

    void Start() {
        globalLight = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
    }

    void FixedUpdate() {
        time += Time.fixedDeltaTime;
        if(time >= k_MORNING) time = 0;
        
        globalLight.intensity = (time < k_DAY)      ? Mathf.Lerp(brightness_night, brightness_day, time/k_DAY)
                              : (time < k_EVENING   ? brightness_day
                              : (time < k_NIGHT     ? Mathf.Lerp(brightness_day, brightness_night, (time-k_EVENING)/(k_NIGHT-k_EVENING))
                              :  brightness_night));
    }
}
