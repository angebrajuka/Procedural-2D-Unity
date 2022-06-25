﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour {
    // hierarchy
    public float fadeSpeed=3.0f;

    public static bool black;
    static RawImage image;

    public void Start() {
        image = GetComponent<RawImage>();
    }

    public void Update() {
        Color col = image.color;
        col.a = Mathf.MoveTowards(col.a, black ? 1 : 0, Time.unscaledDeltaTime * fadeSpeed);
        image.color = col;
    }
}
