using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    public GameObject prevMenu;

    public bool canUseEsc=true;

    void Start() {
        if(MenuHandler.currentMenu == null) MenuHandler.currentMenu = gameObject;
    }

    void Update() {
        if(canUseEsc && Input.GetKeyDown(KeyCode.Escape)) {
            if(prevMenu != null) MenuHandler.SetMenu(prevMenu);
            else if(PlayerStats.playerInput != null) {
                PauseHandler.UnPause();
                PauseHandler.UnBlur();
                Destroy(gameObject);
                MenuHandler.currentMenu = null;
                PlayerStats.playerInput.enabled = true;
            }
        }
    }
}
