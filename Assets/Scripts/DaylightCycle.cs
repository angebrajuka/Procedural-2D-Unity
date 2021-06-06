using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class DaylightCycle : MonoBehaviour {

    enum TimeOfDay:byte {
        MORNING=0,
        DAY=1,
        EVENING=2,
        NIGHT=3
    } static TimeOfDay timeOfDay = TimeOfDay.MORNING;

    float timer = 0;
    static readonly float[] speeds = {0.0027f, 0, -0.0027f, 0};
    static readonly float[] durations = {6, 120, 6, 90};

    UnityEngine.Experimental.Rendering.Universal.Light2D globalLight;

    void Start() {
        globalLight = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
    }

    void FixedUpdate() {
        if(timer <= 0) {
            timeOfDay ++;
            if(timeOfDay > TimeOfDay.NIGHT) timeOfDay = TimeOfDay.MORNING;
            timer = durations[(int)timeOfDay];
        }
        timer -= Time.fixedDeltaTime;
        globalLight.intensity += speeds[(int)timeOfDay];
    }
}
