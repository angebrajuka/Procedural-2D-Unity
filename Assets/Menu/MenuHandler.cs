using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler {
    
    public static GameObject currentMenu;
    public static GameObject menuMain;

    public static void SetMenu(GameObject menu) {
        if(currentMenu != null) MonoBehaviour.Destroy(currentMenu);
        currentMenu = MonoBehaviour.Instantiate(menu);
    }
}
