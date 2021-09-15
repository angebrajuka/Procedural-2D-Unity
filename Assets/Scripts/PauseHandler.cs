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
    static Dictionary<Rigidbody2D, Vector3> rb_vels;
    public static bool paused;

    public void Init()
    {
        volume.profile.TryGet<DepthOfField>(out dofComponent);
        rb_vels = new Dictionary<Rigidbody2D, Vector3>();
        paused = false;
    }

    public static void FreezePhysics()
    {
        rb_vels.Clear();
        Rigidbody2D[] rb = Rigidbody.FindObjectsOfType(typeof(Rigidbody2D)) as Rigidbody2D[];
        foreach(Rigidbody2D body in rb)
        {
            rb_vels.Add(body, body.velocity);
            body.velocity *= 0;
        }
    }

    public static void UnfreezePhysics()
    {
        foreach(KeyValuePair<Rigidbody2D, Vector3> pair in rb_vels)
        {
            pair.Key.velocity = pair.Value;
        }
    }

    public static void DisableInputAndHUD()
    {
        PlayerStats.playerInput.enabled = false;
        PlayerStats.hud.transform.gameObject.SetActive(false);
    }

    public static void EnableInputAndHUD()
    {
        PlayerStats.playerInput.enabled = true;
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
