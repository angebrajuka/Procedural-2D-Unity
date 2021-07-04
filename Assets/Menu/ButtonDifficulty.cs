using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDifficulty : MonoBehaviour
{
    public byte difficulty;

    public void OnClick()
    {
        PlayerStats.difficulty = difficulty;
        PlayerStats.load = false;
        MenuHandler.Start();
    }
}
