using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class Init : MonoBehaviour
{
    // hierarchy
    public GameObject h_menuMain;
    public GameObject h_menuPause;
    public bool setMenu;

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

            PlayerInput.LoadKeybinds();

            MenuHandler.mainMenuObject = mainMenuObject;
            MenuHandler.gameWorldObject = gameWorldObject;
        }

        if(setMenu) MenuHandler.SetMenu(MenuHandler.menuMain);
        
        Destroy(gameObject);
    }
}
