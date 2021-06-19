using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {
    
    // hierarchy
    public Transform weapons;
    public GameObject line;
    public Inventory inventory;


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
    public const int key_moveEast=0, key_moveNorth=1, key_moveWest=2, key_moveSouth=3, key_shoot=4, key_reload=5, key_item=6, key_interact=7, key_drop=8, key_inventory=9, key_i_equip=10, key_i_rotate=11;
    public static KeyCode[] keybinds = new KeyCode[12];
    
    public static void DefaultKeybinds() {
        keybinds[key_moveEast]  = KeyCode.D;
        keybinds[key_moveNorth] = KeyCode.W;
        keybinds[key_moveWest]  = KeyCode.A;
        keybinds[key_moveSouth] = KeyCode.S;
        keybinds[key_shoot]     = KeyCode.Mouse0;
        keybinds[key_reload]    = KeyCode.R;
        keybinds[key_item]      = KeyCode.Mouse1;
        keybinds[key_interact]  = KeyCode.F;
        keybinds[key_drop]      = KeyCode.P;
        keybinds[key_inventory] = KeyCode.Tab;
        keybinds[key_i_equip]   = KeyCode.E;
        keybinds[key_i_rotate]  = KeyCode.R;
    }

    // public static void LoadKeybinds() {
    //     keybinds[key_moveEast]  = (KeyCode)PlayerPrefs.GetInt("key_moveEast");
    //     keybinds[key_moveNorth] = (KeyCode)PlayerPrefs.GetInt("key_moveNorth");
    //     keybinds[key_moveWest]  = (KeyCode)PlayerPrefs.GetInt("key_moveWest");
    //     keybinds[key_moveSouth] = (KeyCode)PlayerPrefs.GetInt("key_moveSouth");
    //     keybinds[key_shoot]     = (KeyCode)PlayerPrefs.GetInt("key_shoot");
    // }

    // public static void SaveKeybinds() {
    //     PlayerPrefs.SetInt("key_moveEast",  (int)keybinds[key_moveEast]);
    //     PlayerPrefs.SetInt("key_moveNorth", (int)keybinds[key_moveNorth]);
    //     PlayerPrefs.SetInt("key_moveWest",  (int)keybinds[key_moveWest]);
    //     PlayerPrefs.SetInt("key_moveSouth", (int)keybinds[key_moveSouth]);
    //     PlayerPrefs.SetInt("key_shoot",     (int)keybinds[key_shoot]);
    // }



    void Start() {
        DefaultKeybinds();
        m_rigidbody = GetComponent<Rigidbody2D>();
        PlayerStats.rigidbody = m_rigidbody;
        PlayerStats.target = GetComponent<Target>();
    }

    void Update() {
        
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

            if(Input.GetKey(keybinds[key_moveEast]))  input_move.x ++;
            if(Input.GetKey(keybinds[key_moveNorth])) input_move.y ++;
            if(Input.GetKey(keybinds[key_moveWest]))  input_move.x --;
            if(Input.GetKey(keybinds[key_moveSouth])) input_move.y --;
            
                // rotate
                if(input_move.x != 0 || input_move.y != 0)
                    direction8index = Math.directions8[(int)-input_move.y+1, (int)input_move.x+1];
                else
                    direction8index = Math.directions8[(int)-Mathf.Round(mouse_offset.y)+1, (int)Mathf.Round(mouse_offset.x)+1];

            input_move.Normalize();

            if(Input.GetKey(KeyCode.Space)) PlayerAnimator.mood = PlayerAnimator.Mood.ANGRY;
            else PlayerAnimator.mood = PlayerAnimator.Mood.HAPPY;
        }


        // inventory
        if(Input.GetKeyDown(keybinds[key_inventory])) {
            PauseHandler.Pause();
            PauseHandler.Blur();
            inventory.gameObject.SetActive(true);
            inventory.Open();
            if(PlayerStats.gunReloadTimer > 0) PlayerStats.CancelReload();
            PlayerStats.hud.transform.gameObject.SetActive(false);
            enabled = false;
        }
        

        // reload
        if(Input.GetKey(keybinds[key_reload])) PlayerStats.BeginReload();

        // pew pew
        if(Input.GetKey(keybinds[key_shoot]) && PlayerStats.CanShoot()) {
            PlayerStats.currentGun.Shoot(m_rigidbody.position, mouse_offset, angle, m_rigidbody);
            PlayerStats.hud.UpdateAmmo();
        }

        //items
        if(Input.GetKeyDown(keybinds[key_item])) {
            Items.items[(int)PlayerStats.currentItem].use();
        }


        // interact
        if(Input.GetKeyDown(keybinds[key_interact])) {
            if(PlayerStats.interactItem != Item.NONE) {
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



    void FixedUpdate() {
        m_rigidbody.AddForce(input_move*PlayerStats.k_RUN_ACCELL);
    }

}