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

public static class AudioManager
{
    public static AudioMixer mixer;
    public static AudioMixerGroup mixer_master;
    public static AudioMixerGroup mixer_music;
    public static AudioMixerGroup mixer_sfx;
    public static AudioMixerGroup mixer_menu;
    public static float volMaster, volMusic, volSFX, volMenu;
    public static float volMenuMusicDiff;


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
        mixer.SetFloat("VolMaster", Mathf.Log10(volMaster) * 20);
        mixer.SetFloat("VolMusic", Mathf.Log10(volMusic*volMenuMusicDiff) * 20);
        mixer.SetFloat("VolSFX", Mathf.Log10(volSFX) * 20);
        mixer.SetFloat("VolMenu", Mathf.Log10(volMenu) * 20);
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
        mixer.SetFloat("PitchSFX", pitch);
    }

    static List<AudioSource> audioSources;
    public static void PauseAllAudio()
    {
        audioSources = new List<AudioSource>(Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[]);
        for(int i=0; i<audioSources.Count; i++)
        {
            if(!audioSources[i].isPlaying || audioSources[i].transform.tag == "Music")
            {
                audioSources.RemoveAt(i);
                i--;
            }
            else
            {
                audioSources[i].Pause();
            }
        }
    }
    public static void ResumeAllAudio()
    {
        if(audioSources != null) foreach(AudioSource a in audioSources)
        {
            if(a != null) a.Play();
        }
    }

    public static GameObject PlayClip(AudioClip clip, float volume, Mixer mixer, float spatialBlend=0.0f, Vector3 position=default(Vector3))
    {
        GameObject gameObject = new GameObject();
        gameObject.transform.position = position;
        AudioSource source = gameObject.AddComponent<AudioSource>();
        switch(mixer)
        {
        case Mixer.SFX:
            source.outputAudioMixerGroup = mixer_sfx;
            break;
        case Mixer.MENU:
            source.outputAudioMixerGroup = mixer_menu;
            break;
        case Mixer.MUSIC:
            source.outputAudioMixerGroup = mixer_music;
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
