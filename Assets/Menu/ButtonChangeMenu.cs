using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChangeMenu : MonoBehaviour {
    
    public GameObject menu;

    public void OnButtonPress() {
        MenuHandler.SetMenu(menu);
    }
}
