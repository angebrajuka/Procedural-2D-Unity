using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Keybind:byte
{
    moveEast,
    moveNorth,
    moveWest,
    moveSouth,
    shoot,
    reload,
    item,
    interact,
    inventory,
    i_equip,
    i_rotate
}

public class PlayerInput : MonoBehaviour
{    
    // hierarchy
    public Transform weapons;
    public GameObject line;
    public InventoryControls inventory;


    // components
    public static Rigidbody2D m_rigidbody;


    // input
    private Vector2 input_move;
    private Vector2 mouse_offset;

    // output
    public static float angle;
    public static float cursorDistance;
    public static byte direction8index;



    // keybinds
    public static Dictionary<Keybind, KeyCode>  keybinds = new Dictionary<Keybind, KeyCode>()
    {
        {Keybind.moveEast,      KeyCode.D       },
        {Keybind.moveNorth,     KeyCode.W       },
        {Keybind.moveWest,      KeyCode.A       },
        {Keybind.moveSouth,     KeyCode.S       },
        {Keybind.shoot,         KeyCode.Mouse0  },
        {Keybind.reload,        KeyCode.R       },
        {Keybind.item,          KeyCode.Mouse1  },
        {Keybind.interact,      KeyCode.F       },
        {Keybind.inventory,     KeyCode.Tab     },
        {Keybind.i_equip,       KeyCode.E       },
        {Keybind.i_rotate,      KeyCode.R       },
    };

    public static Dictionary<Keybind, string>   keybindStrings = new Dictionary<Keybind, string>()
    {
        {Keybind.moveEast,      "moveEast"  },
        {Keybind.moveNorth,     "moveNorth" },
        {Keybind.moveWest,      "moveWest"  },
        {Keybind.moveSouth,     "moveSouth" },
        {Keybind.shoot,         "shoot"     },
        {Keybind.reload,        "reload"    },
        {Keybind.item,          "item"      },
        {Keybind.interact,      "interact"  },
        {Keybind.inventory,     "inventory" },
        {Keybind.i_equip,       "i_equip"   },
        {Keybind.i_rotate,      "i_rotate"  },
    };
    
    public static void DefaultKeybinds()
    {
        keybinds[Keybind.moveEast]  = KeyCode.D;
        keybinds[Keybind.moveNorth] = KeyCode.W;
        keybinds[Keybind.moveWest]  = KeyCode.A;
        keybinds[Keybind.moveSouth] = KeyCode.S;
        keybinds[Keybind.shoot]     = KeyCode.Mouse0;
        keybinds[Keybind.reload]    = KeyCode.R;
        keybinds[Keybind.item]      = KeyCode.Mouse1;
        keybinds[Keybind.interact]  = KeyCode.F;
        keybinds[Keybind.inventory] = KeyCode.Tab;
        keybinds[Keybind.i_equip]   = KeyCode.E;
        keybinds[Keybind.i_rotate]  = KeyCode.R;
    }

    public static void LoadKeybinds()
    {    
        foreach(KeyValuePair<Keybind, string> pair in keybindStrings)
        {
            if(!PlayerPrefs.HasKey(keybindStrings[pair.Key]))
            {
                SaveKeybinds();
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



    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        PlayerStats.rigidbody = m_rigidbody;
        PlayerStats.target = GetComponent<Target>();
    }

    void Update()
    {
        // pause
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseHandler.Pause();
            PauseHandler.Blur();
            enabled = false;
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

            weapons.eulerAngles = new Vector3(0, 0, angle);
        }


        // movement
        {
            input_move.x = 0;
            input_move.y = 0;

            if(Input.GetKey(keybinds[Keybind.moveEast]))  input_move.x ++;
            if(Input.GetKey(keybinds[Keybind.moveNorth])) input_move.y ++;
            if(Input.GetKey(keybinds[Keybind.moveWest]))  input_move.x --;
            if(Input.GetKey(keybinds[Keybind.moveSouth])) input_move.y --;
            
                // rotate
                if(input_move.x != 0 || input_move.y != 0)  direction8index = Math.directions8[(int)-input_move.y+1, (int)input_move.x+1];
                else                                        direction8index = Math.directions8[(int)-Mathf.Round(mouse_offset.y)+1, (int)Mathf.Round(mouse_offset.x)+1];

            input_move.Normalize();

            if(Input.GetKey(KeyCode.Space))     PlayerAnimator.mood = PlayerAnimator.Mood.ANGRY;
            else                                PlayerAnimator.mood = PlayerAnimator.Mood.HAPPY;
        }


        // inventory
        if(Input.GetKeyDown(keybinds[Keybind.inventory]))
        {
            PauseHandler.Pause();
            PauseHandler.Blur();
            inventory.gameObject.SetActive(true);
            PlayerStats.inventory.Open();
            
            if(PlayerStats.gunReloadTimer > 0) PlayerStats.CancelReload();
            
            PlayerStats.hud.transform.gameObject.SetActive(false);
            enabled = false;
        }
        

        // reload
        if(Input.GetKey(keybinds[Keybind.reload])) PlayerStats.BeginReload();

        // pew pew
        if(Input.GetKey(keybinds[Keybind.shoot]) && PlayerStats.CanShoot())
        {
            PlayerStats.currentGun.Shoot(m_rigidbody.position, mouse_offset, angle, m_rigidbody);
            PlayerStats.hud.UpdateAmmo();
        }

        //items
        if(Input.GetKeyDown(keybinds[Keybind.item]))
        {
            Items.items[(int)PlayerStats.currentItem].use();
        }


        // interact
        if(Input.GetKeyDown(keybinds[Keybind.interact]))
        {
            if(PlayerStats.interactItem != Item.NONE)
            {
                if(PlayerStats.inventory.AutoAdd(PlayerStats.interactItem))
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



    void FixedUpdate()
    {
        m_rigidbody.AddForce(input_move*PlayerStats.k_RUN_ACCELL);
    }
}