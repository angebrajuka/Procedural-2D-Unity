using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public bool canUseEsc=true;

    void Start()
    {
        if(MenuHandler.currentMenu == null) MenuHandler.currentMenu = gameObject;
    }

    void Update()
    {
        if(canUseEsc && Input.GetKeyDown(KeyCode.Escape))
        {
            if(!MenuHandler.Back())
            {
                PauseHandler.UnPause();
                PauseHandler.UnBlur();
                Destroy(gameObject);
                MenuHandler.currentMenu = null;
                MenuHandler.currentMenuPrefab = null;
            }
        }
    }
}
