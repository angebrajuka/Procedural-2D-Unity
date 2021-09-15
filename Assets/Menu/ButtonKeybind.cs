using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonKeybind : MonoBehaviour {
    
    public Keybind keybind;
    public Transform overlay;

    private byte selected = 0;
    private Menu menu;

    void Start() {
        SetText(PlayerInput.keybinds[keybind]);
        menu = transform.parent.parent.parent.parent.parent.parent.GetComponent<Menu>();
    }

    void SetText(KeyCode key) {
        Text t = transform.GetChild(0).GetComponent<Text>();
        t.text = "<"+key+">";
    }

    void OnKeyPress(KeyCode key) {
        SetText(key);
        PlayerInput.keybinds[keybind] = key;
        PlayerInput.SaveKeybinds();
        menu.canUseEsc = true;
    }

    public void OnButtonPress() {
        overlay.localScale = new Vector3(1, 1, 1);
        selected = 1;
        menu.canUseEsc = false;
    }

    void Update() {
        if(Input.anyKey && selected == 2) {
            if(Input.GetKey(KeyCode.Escape)) {
                selected = 0;
                overlay.localScale *= 0;
                menu.canUseEsc = true;
            } else {
                for(int i=0; i<670; i++) {
                    if(Input.GetKeyDown((KeyCode)i)) {
                        OnKeyPress((KeyCode)i);
                        selected = 0;
                        overlay.localScale *= 0;
                        break;
                    }
                }
            }
        }
        if(selected == 1 && !Input.GetKey(KeyCode.Mouse0)) selected = 2;
    }
}
