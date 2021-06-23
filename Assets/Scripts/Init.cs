using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class Init : MonoBehaviour {

    // hierarchy
    public AudioMixer h_mixer;
    public AudioMixerGroup h_mixer_master;
    public AudioMixerGroup h_mixer_music;
    public AudioMixerGroup h_mixer_sfx;
    public AudioMixerGroup h_mixer_menu;

    public GameObject h_menuMain;


    static bool init=false;
    
    void Start() {

        if(!init) {
            
            init = true;
            
            AudioManager.mixer = h_mixer;
            AudioManager.mixer_master = h_mixer_master;
            AudioManager.mixer_music = h_mixer_music;
            AudioManager.mixer_sfx = h_mixer_sfx;
            AudioManager.mixer_menu = h_mixer_menu;
            AudioManager.LoadAudioSettings();

            MenuHandler.menuMain = h_menuMain;

            PlayerInput.LoadKeybinds();
        }
        
        Destroy(gameObject);
    }
}
