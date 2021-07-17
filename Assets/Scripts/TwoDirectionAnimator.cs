﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDirectionAnimator : MonoBehaviour
{
    // hierarchy
    public Sprite[] sprites;
    public int state;
    public int direction;
    public SpriteRenderer m_spriteRenderer;

    public void Enable()    { m_spriteRenderer.enabled = true; }
    public void Disable()   { m_spriteRenderer.enabled = false; }

    void Start()
    {
        state = 0;
        direction = 0;
    }

    void Update()
    {
        m_spriteRenderer.sprite = sprites[state];
        m_spriteRenderer.flipX = direction == 1;
    }
}