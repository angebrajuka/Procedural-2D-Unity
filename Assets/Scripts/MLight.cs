using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class MLight : MonoBehaviour {
    [HideInInspector] public Light2D light2D;

    public float brightness;

    void Start() {
        light2D = GetComponent<Light2D>();
    }

    void Update() {
        light2D.intensity = Mathf.Max(0, brightness - DaylightCycle.brightness);
    }
}