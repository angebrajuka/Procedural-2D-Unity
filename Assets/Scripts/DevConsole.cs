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

    public void Init()
    {
        s_target = target;
        Disable();
    }


    static bool Time(string[] args)
    {
        float amount = float.Parse(args[1]);

        switch(args[0])
        {
        case "set":
            DaylightCycle.time = amount;
            break;
        case "add":
            DaylightCycle.time += amount;
            break;
        case "subtract":
            DaylightCycle.time -= amount;
            break;
        default:
            return false;
        }
    
        return true;
    }

    static bool Health(string[] args)
    {
        float amount = float.Parse(args[1]);

        switch(args[0])
        {
        case "add":
            s_target.Heal(amount);
            break;
        case "sub":
            s_target.Damage(amount);
            break;
        default:
            return false;
        }
    
        return true;
    }

    static bool Energy(string[] args)
    {
        float amount = float.Parse(args[1]);

        switch(args[0])
        {
        case "set":
            PlayerStats.energy = amount;
            break;
        case "add":
            PlayerStats.energy += amount;
            break;
        default:
            return false;
        }

        PlayerStats.hud.UpdateEnergy();
    
        return true;
    }

    static bool AutoAddItem(string[] args)
    {
        int itemID = Int32.Parse(args[0]);
        int count = Int32.Parse(args[1]);

        if(itemID >= (int)Item.LAST || itemID <= (int)Item.NONE) return false;

        PlayerStats.inventory.AutoAdd((Item)itemID, count);

        return true;
    }

    static bool SetGun(string[] args)
    {
        PlayerStats.SwitchGun(sbyte.Parse(args[0]), true);
        return true;
    }


    static bool Teleport(string[] args)
    {
        if(Int32.TryParse(args[0], out int x) && Int32.TryParse(args[1], out int y))
        {
            PlayerStats.rigidbody.position = new Vector3(x, y, 0);

            return true;
        }

        return false;
    }



    static readonly Dictionary<string, Func<string[], bool>> commands = new Dictionary<string, Func<string[], bool>>
    {
        {"time",        Time        },
        {"health",      Health      },
        {"energy",      Energy      },
        {"give",        AutoAddItem },
        {"setgun",      SetGun      },
        {"tp",          Teleport    }
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
