using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour {

    public AudioClip clipHover;
    public AudioClip clipClick;

    public void OnEnter() {
        AudioManager.PlayClip(clipHover, 0.5f, Mixer.MENU);
    }

    public void OnClick() {
        AudioManager.PlayClip(clipClick, 1.0f, Mixer.MENU);
    }
}
