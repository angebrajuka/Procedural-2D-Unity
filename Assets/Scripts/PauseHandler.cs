using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseHandler : MonoBehaviour
{
    public Volume volume;
    static DepthOfField dofComponent;
    static int focalLengthVal=1;
    public static bool paused;

    public void Init()
    {
        volume.profile.TryGet<DepthOfField>(out dofComponent);
        paused = false;
    }

    public static void FreezePhysics()
    {
        Time.timeScale = 0;
    }

    public static void UnfreezePhysics()
    {
        Time.timeScale = 1;
    }

    public static void DisableInputAndHUD()
    {
        PlayerStats.playerInput.enabled = false;
        PlayerStats.playerMovement.enabled = false;
        PlayerStats.hud.transform.gameObject.SetActive(false);
    }

    public static void EnableInputAndHUD()
    {
        PlayerStats.playerInput.enabled = true;
        PlayerStats.playerMovement.enabled = true;
        PlayerStats.hud.transform.gameObject.SetActive(true);
    }

    public static void Pause()
    {
        paused = true;

        FreezePhysics();
        DisableInputAndHUD();

        AudioManager.PauseAllAudio();
    }

    public static void UnPause()
    {
        UnfreezePhysics();
        EnableInputAndHUD();

        AudioManager.ResumeAllAudio();

        paused = false;
    }

    public static void Blur()
    {
        focalLengthVal = 40;
        dofComponent.focalLength.value = 15;
    }

    public static void UnBlur()
    {
        focalLengthVal = 1;
    }

    void Update()
    {
        dofComponent.focalLength.value = Mathf.Lerp(dofComponent.focalLength.value, focalLengthVal, Time.unscaledDeltaTime*4);
    }
}
