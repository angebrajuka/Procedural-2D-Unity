using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSave : MonoBehaviour
{
    public void OnClick()
    {
        Save_Load.Save(PlayerStats.save);
    }
}
