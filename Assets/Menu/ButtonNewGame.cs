using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNewGame : MonoBehaviour
{
    public byte save;
    public GameObject difficultyMenu;

    public void OnClick()
    {
        PlayerStats.save = save;
        MenuHandler.SetMenu(difficultyMenu);
    }
}
