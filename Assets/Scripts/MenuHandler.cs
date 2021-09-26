using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler
{
    public static Stack<GameObject> prevMenu = new Stack<GameObject>();
    public static GameObject currentMenu;
    public static GameObject currentMenuPrefab;
    public static GameObject menuMain;
    public static GameObject menuPause;

    public static GameObject mainMenuObject;
    public static GameObject gameWorldObject;

    static void SetMenuRaw(GameObject menu)
    {
        if(currentMenu != null) MonoBehaviour.Destroy(currentMenu);
        currentMenu = MonoBehaviour.Instantiate(menu);
        currentMenuPrefab = menu;
    }

    public static bool Back()
    {
        if(prevMenu.Count > 0)
        { 
            SetMenuRaw(prevMenu.Pop());
            return true;
        }
        return false;
    }

    public static void SetMenu(GameObject menu, bool fade=false)
    {
        if(currentMenuPrefab != null) prevMenu.Push(currentMenuPrefab);

        SetMenuRaw(menu);

        if(menu == menuMain)
        {
            prevMenu.Clear();
        }

        if(fade)
        {
            currentMenu.GetComponent<Menu>().alpha = 0;
        }
    }

    public static void Start()
    {
        FadeTransition.SetAlpha(1);
        currentMenuPrefab = null;
        if(currentMenu != null) MonoBehaviour.Destroy(currentMenu);
        currentMenu = null;
        mainMenuObject.SetActive(false);
        gameWorldObject.SetActive(true);
        PlayerState.instance.Reset(); // if load, set seed should happen in Player reset
        ProceduralGeneration.instance.GenerateMap();
        ProceduralGeneration.GenerateTexture(100);
        PlayerHUD.instance.SetMapImage(ProceduralGeneration.textureBiome);
        PlayerStats.loadingFirstChunks = true;
    }

    public static void MainMenu(bool setMenu=true, bool fade=false)
    {
        gameWorldObject.SetActive(false);
        mainMenuObject.SetActive(true);
        if(setMenu)
        {
            SetMenu(menuMain, true);
        }
        PauseHandler.UnBlur();
    }
}
