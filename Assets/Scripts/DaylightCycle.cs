using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DaylightCycle : MonoBehaviour
{
    public const float k_MORNING=240, k_DAY=15, k_EVENING=135, k_NIGHT=150;
    
    // hierarchy
    public float brightness_day, brightness_night;

    public static float time = k_DAY;
    public static float brightness;

    Light2D globalLight;

    public void Start()
    {
        globalLight = GetComponent<Light2D>();
    }

    void Update()
    {
        time += PauseHandler.paused ? 0 : Time.deltaTime;
        time %= k_MORNING;

        brightness = (time < k_DAY)      ? Mathf.Lerp(brightness_night, brightness_day, time/k_DAY)
                              : (time < k_EVENING   ? brightness_day
                              : (time < k_NIGHT     ? Mathf.Lerp(brightness_day, brightness_night, (time-k_EVENING)/(k_NIGHT-k_EVENING))
                              :  brightness_night));
        globalLight.intensity = brightness;
    }
}
