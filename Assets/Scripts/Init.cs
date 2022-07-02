using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;


public class Init : MonoBehaviour
{
    // public static Init instance;

    // // hierarchy
    // public MusicManager musicManager;
    // public GameObject h_menuMain;
    // public GameObject h_menuPause;
    // public Entities entities;
    // public Transform player;
    // public Transform gunSpriteTransform;
    // public FadeTransition fadeTransition;

    // public GameObject mainMenuObject;
    // public GameObject gameWorldObject;

    // static bool init=false;
    
    // void Start()
    // {
    //     instance = this;

    //     if(!init)
    //     {
    //         init = true;

    //         musicManager.Init();
    //         entities.Init();
    //         fadeTransition.Init();
    //         player.GetComponent<DynamicEnemySpawning>().Init();
    //         player.GetComponent<PlayerTarget>().Init();
    //         player.GetComponent<PlayerStats>().Init();
    //         player.GetComponent<PlayerState>().Init();
    //         Items.Init(gunSpriteTransform);
    //         player.GetComponent<PlayerHUD>().Init();
    //         player.GetComponent<PlayerInput>().Init();
    //         PlayerInput.LoadKeybinds();
    //         player.GetComponent<Inventory>().Init();
    //         player.GetComponent<PlayerMovement>().Init();
    //         player.GetComponent<PlayerAnimator>().Init();



    //         // MenuHandler.MainMenu(false);
    //         gameWorldObject.SetActive(false);
    //         mainMenuObject.SetActive(true);
    //     }

    //     Destroy(gameObject);
    // }

    public void Start() {
        FadeTransition.black = true;
        FadeTransition.Snap();
        MenuEvents.MainMenu();
    }
}
