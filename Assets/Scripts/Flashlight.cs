using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Flashlight : MonoBehaviour
{
    private UnityEngine.Experimental.Rendering.Universal.Light2D flashlight;
    public FlickeringLight flickeringLight;

    void Start()
    {
        flashlight = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
    }

    void Update()
    {
        flashlight.intensity = Mathf.Lerp(flashlight.intensity, PlayerStats.currentItem==Item.FLASHLIGHT ? 0.47f : 0, 0.16f);
        flickeringLight.enabled = PlayerStats.currentItem==Item.FLASHLIGHT;
    }
}
