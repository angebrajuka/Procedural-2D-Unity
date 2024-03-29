﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DaylightCycle : MonoBehaviour {
    [SerializeField] private float brightness_day, brightness_night;
    [SerializeField] private Color color_day, color_night;
    [SerializeField] private WorldLoading worldLoading;

    public const float k_MORNING=240, k_DAY=15, k_EVENING=135, k_NIGHT=150;

    private static float brightness;
    public static float Brightness { get { return brightness; } }
    public static float time = k_DAY;

    Light2D globalLight;

    public void Start() {
        globalLight = GetComponent<Light2D>();
    }

    public void Set(float time) {
        DaylightCycle.time = time;
    }

    void Update() {
        if(!PauseHandler.paused) {
            time += Time.deltaTime;
            time %= k_MORNING;
        }

        if(worldLoading.InDungeon) {
            brightness = Mathf.Lerp(brightness_night, brightness_day, 0.4f);
            globalLight.intensity = brightness;
            globalLight.color = color_night;
            return;
        }

        brightness = (time < k_DAY) ?
            Mathf.Lerp(brightness_night, brightness_day, time/k_DAY)
          : (time < k_EVENING   ? brightness_day
          : (time < k_NIGHT     ? Mathf.Lerp(brightness_day, brightness_night, (time-k_EVENING)/(k_NIGHT-k_EVENING))
          :  brightness_night));
        globalLight.intensity = brightness;

        globalLight.color = (time < k_DAY) ?
            Color.Lerp(color_night, color_day, time/k_DAY)
          : (time < k_EVENING   ? color_day
          : (time < k_NIGHT     ? Color.Lerp(color_day, color_night, (time-k_EVENING)/(k_NIGHT-k_EVENING))
          :  color_night));
    }
}
