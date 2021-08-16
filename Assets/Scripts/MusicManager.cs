using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // components
    AudioSource m_audioSource;

    Dictionary<bool, AudioClip[]> clips;
    int prevClip=-1;
    float timer;

    public void Init()
    {
        m_audioSource = GetComponent<AudioSource>();

        clips = new Dictionary<bool, AudioClip[]>();

        Object[] defaultMusicClips  = Resources.LoadAll("Audio/Music/Default", typeof(AudioClip));
        Object[] scaryMusicClips    = Resources.LoadAll("Audio/Music/Scary", typeof(AudioClip));

        clips[false] = new AudioClip[defaultMusicClips.Length];
        for(int i=0; i<clips[false].Length; i++)
        {
            clips[false][i] = (AudioClip)defaultMusicClips[i];
        }
        clips[true] = new AudioClip[scaryMusicClips.Length];
        for(int i=0; i<clips[true].Length; i++)
        {
            clips[true][i] = (AudioClip)defaultMusicClips[i];
        }

        NewClip();
    }

    void NewClip()
    {
        bool isScary = false;//(float)DynamicEnemySpawning.totalDifficulty / (float)DynamicEnemySpawning.GetDifficultyValue() > 0.4f;
        int clip = Random.Range(0, clips[isScary].Length);
        if(clip == prevClip)
        {
            clip ++;
            if(clip >= clips[isScary].Length) clip = 0;
        }
        prevClip = clip;
        m_audioSource.clip = clips[isScary][clip];
        m_audioSource.Play();
        timer = m_audioSource.clip.length;
    }

    void Update()
    {
        if(!m_audioSource.isPlaying)
        {
            NewClip();
        }
    }
}
