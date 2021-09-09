using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSlot : MonoBehaviour
{
    public bool load;
    public byte save;
    public GameObject menuWorldSettings;
    [HideInInspector] public bool empty = true;

    public void OnClick()
    {
        if(load && !empty)
        {
            PlayerStats.load = true;
            PlayerStats.save = save;
            MenuHandler.Start();
        }
        else if(!load)
        {
            PlayerStats.load = false;
            PlayerStats.save = save;
            MenuHandler.SetMenu(menuWorldSettings);
        }
    }
}
