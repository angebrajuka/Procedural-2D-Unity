using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChangeDifficulty : MonoBehaviour
{
    public RectTransform selected;

    void Start()
    {
        var buttons = transform.GetChild(1);
        for(int i=0; i<buttons.childCount; i++)
        {
            var child = buttons.GetChild(i);
            var button = child.GetComponent<ButtonChangeDifficulty>();
            if(button != null && button.difficulty == PlayerStats.difficulty)
            {
                selected.position = child.position;
                break;
            }
        }
    }
}
