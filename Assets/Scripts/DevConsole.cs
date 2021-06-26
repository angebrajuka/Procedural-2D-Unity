using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour
{
    public InputField inputField;
    public Text textObject;
    public PlayerInput playerInput;
    public Target target;

    [HideInInspector] public bool isActive=false;

    static Target s_target;

    void Start()
    {
        s_target = target;
        Disable();
    }


    static bool Time(string[] args)
    {
        switch(args[0])
        {
        case "set":
            DaylightCycle.time = float.Parse(args[1]);
            break;
        case "add":
            DaylightCycle.time += float.Parse(args[1]);
            break;
        case "subtract":
            DaylightCycle.time -= float.Parse(args[1]);
            break;
        default:
            return false;
        }
    
        return true;
    }

    static bool Health(string[] args)
    {
        switch(args[0])
        {
        case "add":
            s_target.Heal(float.Parse(args[1]));
            break;
        case "subtract":
            s_target.Damage(float.Parse(args[1]));
            break;
        default:
            return false;
        }
    
        return true;
    }

    static bool Ammunition(string[] args)
    {
        Ammo type = Ammo.BULLETS_SMALL;
        
        switch(args[0])
        {
        case "bullets_small":
            type = Ammo.BULLETS_SMALL;
            break;
        case "bullets_big":
            type = Ammo.BULLETS_BIG;
            break;
        case "shells":
            type = Ammo.SHELLS;
            break;
        case "energy":
            type = Ammo.ENERGY;
            break;
        case "max":
            foreach(KeyValuePair<Ammo, int> pair in PlayerStats.maxAmmo)
            {
                PlayerStats.ammo[pair.Key] = pair.Value;
            }
            break;
        default:
            return false;
        }

        if(args[0] != "max")
        {
            PlayerStats.ammo[type] = Int32.Parse(args[1]);
            if(PlayerStats.ammo[type] > PlayerStats.maxAmmo[type]) PlayerStats.ammo[type] = PlayerStats.maxAmmo[type];
        }

        PlayerStats.hud.UpdateAmmo();
        
        return true;
    }

    static bool SetGun(string[] args)
    {
        PlayerStats.SwitchGun(sbyte.Parse(args[0]));
        return true;
    }



    static readonly Dictionary<string, Func<string[], bool>> commands = new Dictionary<string, Func<string[], bool>>
    {
        {"time",        Time        },
        {"health",      Health      },
        {"ammo",        Ammunition  },
        {"setgun",      SetGun      }
    };

    void Enable()
    {
        isActive = true;
        inputField.gameObject.SetActive(true);
        playerInput.enabled = false;
        inputField.ActivateInputField();
        textObject.text = "";
    }

    void Disable()
    {
        isActive = false;
        inputField.gameObject.SetActive(false);
        playerInput.enabled = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            if(!isActive)   Enable();
            else            Disable();
        }
    }

    public void OnCommandEntered()
    {
        string text = textObject.text.ToLower();
        string[] words = text.Split(new char[]{' '}, 2);
        try
        {
            commands[words[0]](words[1].Split(' '));
        }
        catch {}
        
        Disable();
    }
}
