using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;


public class Init : MonoBehaviour {
    public void Start() {
        FadeTransition.black = true;
        FadeTransition.Snap();
        MenuEvents.MainMenu();
    }
}
