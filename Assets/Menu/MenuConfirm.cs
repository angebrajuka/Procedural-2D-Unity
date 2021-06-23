using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuConfirm : Menu {

    void Start() {
        transform.GetChild(2).GetChild(1).GetComponent<ButtonChangeMenu>().menu = prevMenu;
    }
}
