using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLoad : MonoBehaviour {
    
    public byte save;
    [HideInInspector] public bool empty = true;

    public void OnButtonPress()
    {
        if(!empty)
        {
            PlayerStats.load = true;
            PlayerStats.save = save;
            MenuHandler.Start();
        }
    }
}
