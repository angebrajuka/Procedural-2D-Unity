using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HUD_Button_Item : EventTrigger {

    public int item;    

    public override void OnPointerDown(PointerEventData data) {
        if(data.pointerId == -2) {
            PlayerStats._item = item;
            PlayerStats.playerStats.playerHUD.UpdateItems();
        }
    }
}
