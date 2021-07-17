using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour {

    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSFX;
    public Slider sliderMenu;

    void Start() {
        UpdateSliders();
    }

    public void UpdateSliders() {
        sliderMaster.value = AudioManager.volMaster;
        sliderMusic.value = AudioManager.volMusic;
        sliderSFX.value = AudioManager.volSFX;
        sliderMenu.value = AudioManager.volMenu;
    }

    public void OnValueChangedMaster() {
        AudioManager.volMaster = sliderMaster.value;
        AudioManager.UpdateAudioSettings();
    }

    public void OnValueChangedMusic() {
        AudioManager.volMusic = sliderMusic.value;
        AudioManager.UpdateAudioSettings();
    }

    public void OnValueChangedSFX() {
        AudioManager.volSFX = sliderSFX.value;
        AudioManager.UpdateAudioSettings();
    }

    public void OnValueChangedMenu() {
        AudioManager.volMenu = sliderMenu.value;
        AudioManager.UpdateAudioSettings();
    }
}
