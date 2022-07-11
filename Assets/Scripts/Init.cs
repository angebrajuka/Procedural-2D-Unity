using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;


public class Init : MonoBehaviour {
    // hierarchy
    public MenuEvents menuEvents;

    public void Start() {
        FadeTransition.black = true;
        FadeTransition.Snap();
        menuEvents.MainMenu();
    }
}
