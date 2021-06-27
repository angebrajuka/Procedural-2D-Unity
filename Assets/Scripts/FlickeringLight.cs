using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class FlickeringLight : MonoBehaviour
{
new public UnityEngine.Experimental.Rendering.Universal.Light2D light;
    public float radiusRange, intensityRange, timingAverage, timingRange, changeSpeed;
    public float averageRadius, averageIntensity;
    public bool useBeginningRadiusAndIntensity;
    private float timer, interval;
    private float targetIntensity;
    private float targetRadius;

    void Start()
    {
        if(useBeginningRadiusAndIntensity)
        {
            averageIntensity = light.intensity;
            averageRadius = light.pointLightOuterRadius;
        }
        timer = 0;
        interval = 0;
        targetIntensity = averageIntensity;
        targetRadius = averageRadius;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= interval)
        {
            timer = 0;
            targetRadius = averageRadius + Random.value*radiusRange*2 - radiusRange;
            targetIntensity = averageIntensity + Random.value*intensityRange*2 - intensityRange;
            interval = timingAverage + Random.value*timingRange*2 - timingRange;
        }

        light.intensity = Mathf.Lerp(light.intensity, targetIntensity, changeSpeed);
        light.pointLightOuterRadius = Mathf.Lerp(light.pointLightOuterRadius, targetRadius, changeSpeed);
    }
}
