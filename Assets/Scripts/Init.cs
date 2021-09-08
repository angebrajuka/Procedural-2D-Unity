using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;


public class Init : MonoBehaviour
{
    public static Init instance;

    // hierarchy
    public AudioManager audioManager;
    public MusicManager musicManager;
    public GameObject h_menuMain;
    public GameObject h_menuPause;
    public bool setMenu;
    public PlayerStats playerStats;
    public PauseHandler pauseHandler;
    public DevConsole devConsole;
    public DaylightCycle daylightCycle;
    public DynamicEnemySpawning dynamicEnemySpawning;
    public ProceduralGeneration proceduralGeneration;
    public FadeTransition fadeTransition;

    public GameObject mainMenuObject;
    public GameObject gameWorldObject;


    public float seed;
    public bool setSeed;

    static bool init=false;
    
    void Start()
    {
        instance = this;

        if(!init)
        {
            init = true;

            MenuHandler.menuMain = h_menuMain;
            MenuHandler.menuPause = h_menuPause;

            audioManager.Init();
            musicManager.Init();
            pauseHandler.Init();
            devConsole.Init();
            daylightCycle.Init();
            dynamicEnemySpawning.Init();
            fadeTransition.Init();
            playerStats.Init();
            PlayerStats.hud.Init();
            proceduralGeneration.Init();
            PlayerInput.LoadKeybinds();

            MenuHandler.mainMenuObject = mainMenuObject;
            MenuHandler.gameWorldObject = gameWorldObject;
        }

        if(setMenu)
        {
            MenuHandler.SetMenu(MenuHandler.menuMain);
        }
        else
        {
            MenuHandler.Start();
        }

        Destroy(gameObject);
    }
}
