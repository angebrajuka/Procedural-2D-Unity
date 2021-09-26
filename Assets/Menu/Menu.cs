using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public bool canUseEsc=true;
    [HideInInspector] public float alpha=1;

    Image[] buttons;

    void Start()
    {
        if(MenuHandler.currentMenu == null) MenuHandler.currentMenu = gameObject;
        
        if(alpha == 0)
        {
            var bt = transform.Find("Buttons");
            buttons = new Image[bt.childCount];
            for(int i=0; i<bt.childCount; i++)
            {
                buttons[i] = bt.GetChild(i).GetComponent<Image>();
                var c = buttons[i].color;
                c.a = alpha;
                buttons[i].color = c;
            }
        }
    }

    void Update()
    {
        if(alpha < 1)
        {
            alpha += Time.deltaTime*0.5f;
            foreach(var image in buttons)
            {
                var c = image.color;
                c.a = alpha;
                image.color = c;
            }
        }
        if(canUseEsc && Input.GetKeyDown(KeyCode.Escape))
        {
            if(!MenuHandler.Back())
            {
                PauseHandler.UnPause();
                PauseHandler.UnBlur();
                Destroy(gameObject);
                MenuHandler.currentMenu = null;
                MenuHandler.currentMenuPrefab = null;
            }
        }
    }
}
