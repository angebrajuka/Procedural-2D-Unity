﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChangeDifficulty : MonoBehaviour
{
    static readonly string[] descriptions = new string[]
    {
        "it's what it says on the tin, recommended for beginners",
        "bit of a step up from beginner, recommended for those focused on story",
        "the intended difficulty, skilled players can complete this difficulty without dieing",
        "a challenge for skilled players, recommended for repeated playthroughs",
        "no. Just no."
    };

    public byte difficulty;
    public Text text;

    public void OnEnter()
    {
        text.text = descriptions[difficulty];
    }

    public void OnExit()
    {
        text.text = "";
    }

    public void OnClick()
    {
        PlayerStats.difficulty = difficulty;
        Save_Load.Save(PlayerStats.save);
    }
}