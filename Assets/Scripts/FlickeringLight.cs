using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLight : MonoBehaviour {
    [SerializeField] private float radiusRange, intensityRange, timingAverage, timingRange, changeSpeed;
    [SerializeField] private float averageRadius, averageIntensity;
    [SerializeField] private bool useBeginningRadius;
    [SerializeField] private bool useBeginningIntensity;

    public bool on;

    private Light2D light2D;
    private MLight mlight;
    private float timer, interval;
    private float targetIntensity;
    private float targetRadius;

    void Start() {
        light2D = GetComponent<Light2D>();
        mlight = GetComponent<MLight>();

        if(useBeginningRadius) {
            averageRadius = light2D.pointLightOuterRadius;
        }
        if(useBeginningIntensity) {
            averageIntensity = light2D.intensity;
        }
        timer = 0;
        interval = 0;
        targetIntensity = averageIntensity;
        targetRadius = averageRadius;
    }

    void Update() {
        timer += Time.deltaTime;
        if(timer >= interval) {
            timer = 0;
            targetRadius = averageRadius + Random.value*radiusRange*2 - radiusRange;
            targetIntensity = averageIntensity + Random.value*intensityRange*2 - intensityRange;
            interval = timingAverage + Random.value*timingRange*2 - timingRange;
        }

        mlight.brightness = Mathf.MoveTowards(mlight.brightness, on ? targetIntensity : 0, changeSpeed);
        light2D.pointLightOuterRadius = Mathf.MoveTowards(light2D.pointLightOuterRadius, targetRadius, changeSpeed);
    }
}
