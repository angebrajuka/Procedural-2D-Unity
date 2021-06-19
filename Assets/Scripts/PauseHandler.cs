using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseHandler : MonoBehaviour {

    static DepthOfField dofComponent;
    static float prevTimeScale = 1;
    static int focalLengthVal=1;

    void Start() {
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet<DepthOfField>(out dofComponent);
    }

    public static void Pause() {
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0;
        
    }

    public static void UnPause() {
        Time.timeScale = prevTimeScale;
    }

    public static void Blur() {
        focalLengthVal = 30;
        dofComponent.focalLength.value = 15;
    }

    public static void UnBlur() {
        focalLengthVal = 1;
    }

    void Update() {
        dofComponent.focalLength.value = Mathf.Lerp(dofComponent.focalLength.value, focalLengthVal, Time.unscaledDeltaTime*4);
    }
}
