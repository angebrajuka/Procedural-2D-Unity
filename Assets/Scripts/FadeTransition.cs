﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class FadeTransition : MonoBehaviour {
    [SerializeField] private float fadeSpeed=3.0f;

    public static bool black;
    private static RawImage image;

    public void Start() {
        image = GetComponent<RawImage>();
    }

    public static bool Done { get { return image.color.a == (black ? 1 : 0); } }

    public static async Task AwaitFade() {
        while(!FadeTransition.Done) await Task.Yield();
    }

    public static void Snap() {
        Color col = image.color;
        col.a = black ? 1 : 0;
        image.color = col;
    }

    public void Update() {
        Color col = image.color;
        col.a = Mathf.MoveTowards(col.a, black ? 1 : 0, Time.unscaledDeltaTime * fadeSpeed);
        image.color = col;
    }
}
