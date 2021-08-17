using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    static Image image;

    public static float fadeSpeed=3.0f;
    public static bool fadingIn=false;
    public static bool done=true;

    static Func<bool> onFadeComplete;

    public void Init()
    {
        image = GetComponent<Image>();
    }

    public static void Fade(bool fadeIn, Func<bool> _onFadeComplete)
    {
        Color col = image.color;
        col.a = fadeIn ? 1 : 0;
        image.color = col;
        done = false;
        fadingIn = fadeIn;
        onFadeComplete = _onFadeComplete;
    }

    public static void SetAlpha(float alpha)
    {
        Color col = image.color;
        col.a = alpha;
        image.color = col;
    }

    public void Update()
    {
        if(!done)
        {
            Color col = image.color;
            col.a += (fadingIn ? -1 : 1) * Time.deltaTime * fadeSpeed;
            if((fadingIn && col.a <= 0) || (!fadingIn && col.a >= 1))
            {
                col.a = Mathf.Round(col.a);
                done = true;
                if(onFadeComplete != null) onFadeComplete();
            }
            image.color = col;
        }
    }
}
