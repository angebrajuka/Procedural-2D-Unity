using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour {

    public InputField inputField;
    public Text textObject;
    public PlayerInput playerInput;

    [HideInInspector] public bool isActive=false;



    static bool Time(string[] args) {
        switch(args[0]) {
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

    static bool Health(string[] args) {
        switch(args[0]) {
        case "add":
            PlayerStats.playerTarget.Heal(float.Parse(args[1]));
            break;
        case "subtract":
            PlayerStats.playerTarget.Damage(float.Parse(args[1]));
            break;
        default:
            return false;
        }
    
        return true;
    }

    static bool Ammunition(string[] args) {

        Ammo type = Ammo.BULLETS;
        
        switch(args[0]) {
        case "bullets":
            type = Ammo.BULLETS;
            break;
        case "shells":
            type = Ammo.SHELLS;
            break;
        case "energy":
            type = Ammo.ENERGY;
            break;
        case "max":
            foreach(KeyValuePair<Ammo, int> pair in PlayerStats.maxAmmo) {
                PlayerStats.ammo[pair.Key] = pair.Value;
            }
            break;
        default:
            return false;
        }

        if(args[0] != "max") {
            PlayerStats.ammo[type] = Int32.Parse(args[1]);
            if(PlayerStats.ammo[type] > PlayerStats.maxAmmo[type]) PlayerStats.ammo[type] = PlayerStats.maxAmmo[type];
        }

        PlayerStats.playerStats.playerHUD.UpdateAmmo();
        
        return true;
    }



    static readonly Dictionary<string, Func<string[], bool>> commands = new Dictionary<string, Func<string[], bool>>{
        {"time",        Time        },
        {"health",      Health      },
        {"ammo",        Ammunition  }
    };

    void Clear() {
        inputField.ActivateInputField();
        textObject.text = "";
    }

    void Update() {

        if(Input.GetKeyDown(KeyCode.BackQuote)) {
            isActive = !isActive;
            inputField.gameObject.SetActive(isActive);
            playerInput.enabled = !isActive;
            if(isActive) Clear();
        }
    }

    public void OnCommandEntered() {
        string text = textObject.text.ToLower();
        string[] words = text.Split(new char[]{' '}, 2);
        try { commands[words[0]](words[1].Split(' ')); } catch {}
        Clear();
    }
}
