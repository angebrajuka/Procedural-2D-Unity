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

    [HideInInspector] public bool isActive=false;

    public void Init()
    {
        Disable();
    }


    static bool Time(string[] args)
    {
        float amount = float.Parse(args[2]);

        switch(args[1])
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
        float amount = float.Parse(args[2]);

        switch(args[1])
        {
        case "add":
            PlayerTarget.target.Heal(amount);
            break;
        case "sub":
            PlayerTarget.target.Damage(amount);
            break;
        default:
            return false;
        }
    
        return true;
    }

    static bool Energy(string[] args)
    {
        float amount = float.Parse(args[2]);

        switch(args[1])
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

        PlayerHUD.instance.UpdateEnergy();
    
        return true;
    }

    static bool AutoAddItem(string[] args)
    {
        string itemID = args[1];
        int count = Int32.Parse(args[2]);

        if(!Items.items.ContainsKey(itemID)) return false;

        Inventory.instance.AutoAdd(itemID, count);

        return true;
    }

    static bool SetGun(string[] args)
    {
        PlayerState.SwitchGun(args[1], true);
        return true;
    }


    static bool Teleport(string[] args)
    {
        if(Int32.TryParse(args[1], out int x) && Int32.TryParse(args[2], out int y))
        {
            PlayerMovement.rb.position = new Vector3(x, y, 0);

            return true;
        }

        return false;
    }

    static bool KFA(string[] args)
    {
        foreach(var pair in Items.guns)
        {
            if(args.Length != 2)
            {
                AutoAddItem(new string[]{"give", pair.Key, "1"});
            }
            AutoAddItem(new string[]{"give", pair.Value.ammoType+"", pair.Value.clipSize+""});
        }

        return true;
    }



    static readonly Dictionary<string, Func<string[], bool>> commands = new Dictionary<string, Func<string[], bool>>
    {
        {"time",        Time        },
        {"health",      Health      },
        {"energy",      Energy      },
        {"give",        AutoAddItem },
        {"setgun",      SetGun      },
        {"tp",          Teleport    },
        {"kfa",         KFA         }
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
        string[] words = text.Split(' ');
        try
        {
            commands[words[0]](words);
        }
        catch {}
        
        Disable();
    }
}
