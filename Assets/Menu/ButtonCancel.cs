using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCancel : MonoBehaviour {
    
    public void OnButtonPress() {
        MenuHandler.Back();
    }
}
