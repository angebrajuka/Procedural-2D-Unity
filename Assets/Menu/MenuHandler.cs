using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler {
    
    public static Stack<GameObject> prevMenu = new Stack<GameObject>();
    public static GameObject currentMenu;
    public static GameObject currentMenuPrefab;
    public static GameObject menuMain;
    public static GameObject menuPause;

    static void SetMenuRaw(GameObject menu) {
        if(currentMenu != null) MonoBehaviour.Destroy(currentMenu);
        currentMenu = MonoBehaviour.Instantiate(menu);
        currentMenuPrefab = menu;
    }

    public static bool Back() {
        if(prevMenu.Count > 0) { 
            SetMenuRaw(prevMenu.Pop());
            return true;
        }
        return false;
    }

    public static void SetMenu(GameObject menu) {
        if(currentMenuPrefab != null) prevMenu.Push(currentMenuPrefab);

        SetMenuRaw(menu);

        if(menu == menuMain) {
            prevMenu.Clear();
        }
        
        currentMenuPrefab = menu;
    }
}
