using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HUD_Button_Gun : EventTrigger {

    public int gun;    

    public override void OnPointerDown(PointerEventData data) {
        if(data.pointerId == -2)
            PlayerStats._nextGun = gun;
    }
}
