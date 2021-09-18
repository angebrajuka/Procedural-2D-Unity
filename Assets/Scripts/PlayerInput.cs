using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerState;

public enum Keybind:byte
{
    moveEast,
    moveNorth,
    moveWest,
    moveSouth,
    sprint,
    flashlight,
    shoot,
    reload,
    interact,
    inventory,
    i_equip,
    i_rotate
}

public class PlayerInput : MonoBehaviour
{    
    // hierarchy
    public InventoryControls inventory;

    // input
    [HideInInspector] public static Vector2 input_move;
    private Vector2 mouse_offset;

    // output
    public static float angle=0;
    public static float cursorDistance=0;


    // keybinds
    public static Dictionary<Keybind, KeyCode> keybinds = null;

    public static Dictionary<Keybind, string> keybindStrings = new Dictionary<Keybind, string>()
    {
        {Keybind.moveEast,      "move east"             },
        {Keybind.moveNorth,     "move north"            },
        {Keybind.moveWest,      "move west"             },
        {Keybind.moveSouth,     "move south"            },
        {Keybind.sprint,        "sprint"                },
        {Keybind.flashlight,    "flashlight"            },
        {Keybind.shoot,         "shoot"                 },
        {Keybind.reload,        "reload"                },
        {Keybind.interact,      "interact"              },
        {Keybind.inventory,     "open inventory"        },
        {Keybind.i_equip,       "inventory equip item"  },
        {Keybind.i_rotate,      "inventory rotate item" },
    };
    
    public static void DefaultKeybinds()
    {
        keybinds = new Dictionary<Keybind, KeyCode>()
        {
            {Keybind.moveEast,      KeyCode.D           },
            {Keybind.moveNorth,     KeyCode.W           },
            {Keybind.moveWest,      KeyCode.A           },
            {Keybind.moveSouth,     KeyCode.S           },
            {Keybind.sprint,        KeyCode.LeftControl },
            {Keybind.flashlight,    KeyCode.LeftShift   },
            {Keybind.shoot,         KeyCode.Mouse0      },
            {Keybind.reload,        KeyCode.R           },
            {Keybind.interact,      KeyCode.F           },
            {Keybind.inventory,     KeyCode.Tab         },
            {Keybind.i_equip,       KeyCode.E           },
            {Keybind.i_rotate,      KeyCode.R           },
        };
    }

    public static void LoadKeybinds()
    {
        if(keybinds == null)
        {
            DefaultKeybinds();
        }
        foreach(KeyValuePair<Keybind, string> pair in keybindStrings)
        {
            if(!PlayerPrefs.HasKey(keybindStrings[pair.Key]))
            {
                SaveKeybinds(true, pair.Key);
            }
            keybinds[pair.Key] = (KeyCode)PlayerPrefs.GetInt(keybindStrings[pair.Key]);
        }
    }

    public static void SaveKeybinds(bool one=false, Keybind bind=Keybind.moveEast)
    {
        if(one)
        {
            PlayerPrefs.SetInt(keybindStrings[bind], (int)keybinds[bind]);
        }
        else
        {
            foreach(KeyValuePair<Keybind, string> pair in keybindStrings)
            {
                PlayerPrefs.SetInt(keybindStrings[pair.Key], (int)keybinds[pair.Key]);
            }
        }
    }

    void Update()
    {
        // pause
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseHandler.Pause();
            PauseHandler.Blur();
            MenuHandler.prevMenu.Clear();
            MenuHandler.SetMenu(MenuHandler.menuPause);
        }
        
        // mouse look
        {
            AspectRatio.halfScreen.x = Screen.width/2;
            AspectRatio.halfScreen.y = Screen.height/2;

            mouse_offset = Input.mousePosition-AspectRatio.halfScreen;
            cursorDistance = mouse_offset.magnitude;
            mouse_offset.Normalize();
            angle = Math.NormalizedVecToAngle(mouse_offset);
        }
        
        shooting = PlayerStats.gunRpmTimer > 0;
        // pew pew
        if(Input.GetKey(keybinds[Keybind.shoot]) && PlayerStats.currentItem != null)
        {
            if(PlayerStats.currentItem.gun != null && PlayerState.CanShoot())
            {
                shooting = true;
                if(PlayerStats.gunRpmTimer <= 0)
                {
                    PlayerStats.currentItem.gun.Shoot(PlayerStats.rigidbody.position, mouse_offset, angle, PlayerStats.rigidbody);
                    PlayerStats.hud.UpdateAmmo();
                }
            }
            else if(!PlayerStats.melee && Input.GetKeyDown(keybinds[Keybind.shoot]))
            {
                PlayerStats.currentItem.Use();
            }
        }

        // movement
        {
            input_move.x = 0;
            input_move.y = 0;

            if(!shooting)
            {
                if(Input.GetKey(keybinds[Keybind.moveEast]))  input_move.x ++;
                if(Input.GetKey(keybinds[Keybind.moveNorth])) input_move.y ++;
                if(Input.GetKey(keybinds[Keybind.moveWest]))  input_move.x --;
                if(Input.GetKey(keybinds[Keybind.moveSouth])) input_move.y --;
                
                input_move.Normalize();
            }
        }

        // facing
        {
            if(input_move.x == 0)
            {
                PlayerAnimator.direction = Input.mousePosition.x > (Screen.width / 2) ? 0 : 1;
            }
            else
            {
                PlayerAnimator.direction = input_move.x > 0 ? 0 : 1;
            }
        }

        // battery consumption
        PlayerState.sprinting = Input.GetKey(keybinds[Keybind.sprint]) && PlayerStats.energy > 0;
        Flashlight.on = !PlayerState.sprinting && PlayerStats.energy > 0 && (Input.GetKeyDown(keybinds[Keybind.flashlight]) ? !Flashlight.on : Flashlight.on);

        // inventory
        if(Input.GetKeyDown(keybinds[Keybind.inventory]))
        {
            PlayerState.EndMelee();
            PauseHandler.Blur();
            PauseHandler.DisableInputAndHUD();
            inventory.gameObject.SetActive(true);
            PlayerStats.inventory.Open();
            
            if(PlayerStats.gunReloadTimer > 0) PlayerState.CancelReload();
        }
        
        // reload
        if(Input.GetKey(keybinds[Keybind.reload])) PlayerState.BeginReload();

        // interact
        if(Input.GetKeyDown(keybinds[Keybind.interact]))
        {
            if(PlayerStats.interactItem != null)
            {
                if(PlayerStats.inventory.AutoAdd(PlayerStats.interactItem, PlayerStats.interactPickup.count, PlayerStats.interactPickup.ammo))
                {
                    Destroy(PlayerStats.interactPickup.gameObject);
                    PlayerStats.interactPickup = null;
                }
                else
                {
                    // message or open inventory?
                }

                // if(PlayerStats.currentItem == Item.NONE) {
                //     PlayerStats.currentItem = PlayerStats.interactItem;
                //     PlayerStats.playerStats.playerHUD.UpdateHotbar();
                //     Destroy(PlayerStats.interactPickup.gameObject);
                //     PlayerStats.interactPickup = null;
                // } else {
                //     //loop through inventory
                // }
                // for(int i=0; i < PlayerStats.hotbar.Length; i++) {
                //     if(PlayerStats.hotbar[i] == Item.NONE) {
                //         PlayerStats.hotbar[i] = PlayerStats.interactItem;
                //         PlayerStats.currentItem = PlayerStats.hotbar[i];
                //         PlayerStats.playerStats.playerHUD.UpdateHotbar();
                //         Destroy(PlayerStats.interactPickup.gameObject);
                //         PlayerStats.interactPickup = null;
                //         break;
                //     }
                // }
            }
        }
    }
}