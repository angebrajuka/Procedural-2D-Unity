using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MenuNewGameLoadGame : MonoBehaviour
{
    void Start()
    {
        Transform buttons = transform.GetChild(0);
        for(int i=0; i<buttons.childCount; i++)
        {
            Transform button = buttons.GetChild(i);
            Text text = button.GetChild(0).GetComponent<Text>();
            DateTime lastPlayed;
            if(Save_Load.GetSaveInfo(i, out lastPlayed))
            {
                text.text = text.text + "    last played:  " + lastPlayed.ToString();
            }
        }
    }
}
