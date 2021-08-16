using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class Init : MonoBehaviour
{
    // hierarchy
    public AudioManager audioManager;
    public MusicManager musicManager;
    public GameObject h_menuMain;
    public GameObject h_menuPause;
    public bool setMenu;
    public PlayerStats playerStats;
    public PauseHandler pauseHandler;
    public DevConsole devConsole;

    public GameObject mainMenuObject;
    public GameObject gameWorldObject;


    static bool init=false;
    
    void Start()
    {
        if(!init)
        {
            init = true;

            MenuHandler.menuMain = h_menuMain;
            MenuHandler.menuPause = h_menuPause;

            audioManager.Init();
            musicManager.Init();
            pauseHandler.Init();
            devConsole.Init();

            playerStats.Init();
            PlayerInput.LoadKeybinds();

            MenuHandler.mainMenuObject = mainMenuObject;
            MenuHandler.gameWorldObject = gameWorldObject;
        }

        if(setMenu) MenuHandler.SetMenu(MenuHandler.menuMain);
        
        Destroy(gameObject);
    }
}
