using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Mixer {
    MUSIC,
    SFX,
    MENU
}

public class AudioManager : MonoBehaviour {

    // hierarchy
    public AudioMixer mixer;
    public AudioMixerGroup mixer_master;
    public AudioMixerGroup mixer_music;
    public AudioMixerGroup mixer_sfx;
    public AudioMixerGroup mixer_menu;

    
    private static Transform player;
    public static AudioManager manager;
    public static float volMaster, volMusic, volSFX, volMenu;
    public static float volMenuMusicDiff;


    void Start() {
        player = transform;
        manager = this;
    }


    public static void DefaultSettings() {
        volMaster = 0.5f;
        volMusic = 0.5f;
        volSFX = 0.5f;
        volMenu = 0.5f;
        volMenuMusicDiff = 1.0f;
    }

    public static void UpdateAudioSettings() {
        manager.mixer.SetFloat("VolMaster", Mathf.Log10(volMaster) * 20);
        manager.mixer.SetFloat("VolMusic", Mathf.Log10(volMusic*volMenuMusicDiff) * 20);
        manager.mixer.SetFloat("VolSFX", Mathf.Log10(volSFX) * 20);
        manager.mixer.SetFloat("VolMenu", Mathf.Log10(volMenu) * 20);
        SaveAudioSettings();
    }

    public static void LoadAudioSettings() {
        volMaster = PlayerPrefs.GetFloat("VolMaster");
        volMusic = PlayerPrefs.GetFloat("VolMusic");
        volSFX = PlayerPrefs.GetFloat("VolSFX");
        volMenu = PlayerPrefs.GetFloat("VolMenu");
        UpdateAudioSettings();
    }

    public static void SaveAudioSettings() {
        PlayerPrefs.SetFloat("VolMaster", volMaster);
        PlayerPrefs.SetFloat("VolMusic", volMusic);
        PlayerPrefs.SetFloat("VolSFX", volSFX);
        PlayerPrefs.SetFloat("VolMenu", volMenu);
    }

    public static void PitchShift(float pitch) {
        manager.mixer.SetFloat("PitchSFX", pitch);
    }

    public static void PlayClip(Vector3 position, AudioClip clip, float volume, Mixer mixer, float spatialBlend=0.0f) {
        GameObject gameObject = new GameObject();
        gameObject.transform.position = position;
        AudioSource source = gameObject.AddComponent<AudioSource>();
        switch(mixer) {
        case Mixer.SFX:
            source.outputAudioMixerGroup = manager.mixer_sfx;
            break;
        case Mixer.MENU:
            source.outputAudioMixerGroup = manager.mixer_menu;
            break;
        case Mixer.MUSIC:
            source.outputAudioMixerGroup = manager.mixer_music;
            break;
        }
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        source.Play();
        Destroy(gameObject, clip.length+0.1f);
    }
}
