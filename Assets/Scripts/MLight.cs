using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class MLight : MonoBehaviour {
    [SerializeField] public float brightness; // inspector or controlled by other scripts

    private Light2D light2D;

    void Start() {
        light2D = GetComponent<Light2D>();
    }

    void Update() {
        light2D.intensity = Mathf.Max(0, brightness - DaylightCycle.Brightness);
    }
}