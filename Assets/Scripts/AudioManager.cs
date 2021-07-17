using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Mixer
{
    MUSIC,
    SFX,
    MENU
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public static float volMaster, volMusic, volSFX, volMenu;
    public static float volMenuMusicDiff;

    // hierarchy
    public AudioMixer mixer;
    public AudioMixerGroup mixer_master;
    public AudioMixerGroup mixer_music;
    public AudioMixerGroup mixer_sfx;
    public AudioMixerGroup mixer_menu;

    void Start()
    {
        instance = this;
        LoadAudioSettings();
    }

    public static void DefaultSettings()
    {
        volMaster = 0.5f;
        volMusic = 0.5f;
        volSFX = 0.5f;
        volMenu = 0.5f;
        volMenuMusicDiff = 1.0f;
    }

    public static void UpdateAudioSettings()
    {
        instance.mixer.SetFloat("VolMaster", Mathf.Log10(volMaster) * 20);
        instance.mixer.SetFloat("VolMusic", Mathf.Log10(volMusic*volMenuMusicDiff) * 20);
        instance.mixer.SetFloat("VolSFX", Mathf.Log10(volSFX) * 20);
        instance.mixer.SetFloat("VolMenu", Mathf.Log10(volMenu) * 20);
        SaveAudioSettings();
    }

    public static void LoadAudioSettings()
    {
        try
        {
            volMaster = PlayerPrefs.GetFloat("VolMaster");
            volMusic = PlayerPrefs.GetFloat("VolMusic");
            volSFX = PlayerPrefs.GetFloat("VolSFX");
            volMenu = PlayerPrefs.GetFloat("VolMenu");
        }
        catch
        {
            DefaultSettings();
        }
        UpdateAudioSettings();
    }

    public static void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("VolMaster", volMaster);
        PlayerPrefs.SetFloat("VolMusic", volMusic);
        PlayerPrefs.SetFloat("VolSFX", volSFX);
        PlayerPrefs.SetFloat("VolMenu", volMenu);
    }

    public static void PitchShift(float pitch)
    {
        instance.mixer.SetFloat("PitchSFX", pitch);
    }

    public static void PauseAllAudio()
    {
        for(int i=0; i<instance.transform.childCount; i++)
        {
            instance.transform.GetChild(i).GetComponent<AudioSource>().Pause();
        }
    }
    public static void ResumeAllAudio()
    {
        for(int i=0; i<instance.transform.childCount; i++)
        {
            instance.transform.GetChild(i).GetComponent<AudioSource>().Play();
        }
    }

    public static GameObject PlayClip(AudioClip clip, float volume, Mixer mixer, float spatialBlend=0.0f, Vector3 position=default(Vector3))
    {
        GameObject gameObject = new GameObject();
        gameObject.transform.parent = instance.transform;
        gameObject.transform.position = position;
        AudioSource source = gameObject.AddComponent<AudioSource>();
        switch(mixer)
        {
        case Mixer.SFX:
            source.outputAudioMixerGroup = instance.mixer_sfx;
            break;
        case Mixer.MENU:
            source.outputAudioMixerGroup = instance.mixer_menu;
            break;
        case Mixer.MUSIC:
            source.outputAudioMixerGroup = instance.mixer_music;
            break;
        }
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        source.Play();
        MonoBehaviour.Destroy(gameObject, clip.length+0.1f);
        return gameObject;
    }
}
