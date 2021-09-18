using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class DaylightCycle : MonoBehaviour
{
    public const float k_MORNING=240, k_DAY=15, k_EVENING=135, k_NIGHT=150;
    
    // hierarchy
    public float brightness_day, brightness_night;

    public static float time = k_DAY;

    UnityEngine.Experimental.Rendering.Universal.Light2D globalLight;

    public void Init()
    {
        globalLight = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
    }

    void Update()
    {
        time += PauseHandler.paused ? 0 : Time.deltaTime;
        if(time >= k_MORNING) time = 0;
        
        globalLight.intensity = (time < k_DAY)      ? Mathf.Lerp(brightness_night, brightness_day, time/k_DAY)
                              : (time < k_EVENING   ? brightness_day
                              : (time < k_NIGHT     ? Mathf.Lerp(brightness_day, brightness_night, (time-k_EVENING)/(k_NIGHT-k_EVENING))
                              :  brightness_night));
    }
}
