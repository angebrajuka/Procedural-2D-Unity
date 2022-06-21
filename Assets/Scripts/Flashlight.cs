using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flashlight : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D flashlight;
    public FlickeringLight flickeringLight;

    public static bool on=false;

    void Start()
    {
        flashlight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    void Update()
    {
        flashlight.intensity = Mathf.Lerp(flashlight.intensity, on ? 0.47f : 0, 0.16f);
        flickeringLight.enabled = on;
    }
}
