using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonConfirmQuit : MonoBehaviour {

    public void OnConfirm() {
        Application.Quit();
    }
}
