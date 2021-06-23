using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryControls : MonoBehaviour {
    
    public PlayerInput playerInput;
    public Inventory inventory;
    


    void Update() {    
        // highlight.localPosition = Vector3.down*(PlayerStats._nextGun-1)*120+Vector3.left*160;

        if(Input.GetKeyDown(PlayerInput.keybinds[Keybind.inventory]) || Input.GetKeyDown(KeyCode.Escape)) {
            inventory.Close();
            PauseHandler.UnPause();
            PauseHandler.UnBlur();
            playerInput.enabled = true;
            PlayerStats.hud.transform.gameObject.SetActive(true);
            transform.gameObject.SetActive(false);
        }
    }
}
