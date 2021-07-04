using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNewGame : MonoBehaviour
{
    public int save;
    public GameObject difficultyMenu;

    public void OnClick()
    {
        PlayerStats.save = (byte)save;
        MenuHandler.SetMenu(difficultyMenu);
    }
}
