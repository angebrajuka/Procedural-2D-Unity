using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryControls : MonoBehaviour {
    
    public PlayerInput playerInput;
    public Inventory inventory;
    


    void Update()
    {
        if(Input.GetKeyDown(PlayerInput.keybinds[Keybind.inventory]) || Input.GetKeyDown(KeyCode.Escape)) {
            inventory.Close();
            PauseHandler.blurred = false;
            // PauseHandler.EnableInputAndHUD();
            playerInput.enabled = true;
            transform.gameObject.SetActive(false);
        }
    }
}
