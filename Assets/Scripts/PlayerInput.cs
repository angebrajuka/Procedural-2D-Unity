﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {
    
    // hierarchy
    public Transform weapons;
    public Transform prefab_bomb;
    public GameObject weaponWheel;
    public Transform weaponWheelHighlight;


    // components
    new public static Rigidbody2D rigidbody;
    [HideInInspector] public Image wheelHighlightRenderer;


    // input
    private Vector2 input_move;
    private Vector2 mouse_offset;
    private bool isWheelActive=false;
    // output
    public static float angle;
    public static byte direction8index;



    // keybinds
    public const int key_moveEast=0, key_moveNorth=1, key_moveWest=2, key_moveSouth=3, key_shoot=4, key_reload=5, key_item=6, key_interact=7, key_drop=8, key_wheel=9, key_weapon_0=10, key_weapon_1=11, key_weapon_2=12, key_weapon_3=13, key_weapon_4=14, key_weapon_5=15;
    public static KeyCode[] keybinds = new KeyCode[16];
    
    public static void DefaultKeybinds() {
        keybinds[key_moveEast]  = KeyCode.D;
        keybinds[key_moveNorth] = KeyCode.W;
        keybinds[key_moveWest]  = KeyCode.A;
        keybinds[key_moveSouth] = KeyCode.S;
        keybinds[key_shoot]     = KeyCode.Mouse0;
        keybinds[key_reload]    = KeyCode.R;
        keybinds[key_item]      = KeyCode.Mouse1;
        keybinds[key_interact]  = KeyCode.E;
        keybinds[key_drop]      = KeyCode.P;
        keybinds[key_wheel]     = KeyCode.Mouse2;
        keybinds[key_weapon_0]  = KeyCode.Alpha1;
        keybinds[key_weapon_1]  = KeyCode.Alpha2;
        keybinds[key_weapon_2]  = KeyCode.Alpha3;
        keybinds[key_weapon_3]  = KeyCode.Alpha4;
        keybinds[key_weapon_4]  = KeyCode.Alpha5;
        keybinds[key_weapon_5]  = KeyCode.Alpha6;
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
        rigidbody = GetComponent<Rigidbody2D>();
        wheelHighlightRenderer = weaponWheelHighlight.GetComponent<Image>();
    }

    void RemoveCurrentItem() {
        PlayerStats.hotbar[PlayerStats._item] = Item.NONE;
        PlayerStats.currentItem = Item.NONE;
        PlayerStats.playerStats.playerHUD.UpdateHotbar();
    }

    static readonly Vector2[] wheelPositions = {    new Vector2(2,-3),
                                                    new Vector2(4, 0),
                                                    new Vector2(-2, -3),
                                                    new Vector2(-4, 0),
                                                    new Vector2(2, 3),
                                                    new Vector2(-2, 3) };

    static readonly byte[] convert = {3, 2, 0, 1, 4, 5};

    void Update() {
        
        // mouse look
        {
            AspectRatio.halfScreen.x = Screen.width/2;
            AspectRatio.halfScreen.y = Screen.height/2;

            mouse_offset = Input.mousePosition-AspectRatio.halfScreen;
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


        // switching (wheel)
        if(Input.GetKey(keybinds[key_wheel])) {
            if(!isWheelActive) {
                weaponWheel.transform.position = new Vector3(Input.mousePosition.x+wheelPositions[PlayerStats._currentGun].x, Input.mousePosition.y+wheelPositions[PlayerStats._currentGun].y, 0);
                isWheelActive = true;
                weaponWheel.SetActive(true);
            }

            Vector3 wheelOffset = Input.mousePosition-weaponWheel.transform.position;
            float angle = Math.VecToAngle(wheelOffset);

            int preConvert = (int)Mathf.Floor((angle+30)/60);
            if(preConvert > 5) preConvert = 0;

            wheelHighlightRenderer.transform.eulerAngles = new Vector3(0, 0, preConvert*60);

            PlayerStats._nextGun = convert[preConvert];
        } else if(isWheelActive){
            weaponWheel.SetActive(false);
            isWheelActive = false;
        }

        // item switching (scroll)
        if(Input.mouseScrollDelta.y != 0) {
            if(Input.mouseScrollDelta.y > 0) {
                PlayerStats._item --;
                if(PlayerStats._item < 0) PlayerStats._item = 11;
            } else {
                PlayerStats._item ++;
                if(PlayerStats._item > 11) PlayerStats._item = 0;
            }
            PlayerStats.currentItem = PlayerStats.hotbar[PlayerStats._item];
            PlayerStats.playerStats.playerHUD.UpdateHotbar();
        }

        // weapon switching (keybinds)
        if(Input.GetKey(keybinds[key_weapon_0])) PlayerStats._nextGun = 0;
        if(Input.GetKey(keybinds[key_weapon_1])) PlayerStats._nextGun = 1;
        if(Input.GetKey(keybinds[key_weapon_2])) PlayerStats._nextGun = 2;
        if(Input.GetKey(keybinds[key_weapon_3])) PlayerStats._nextGun = 3;
        if(Input.GetKey(keybinds[key_weapon_4])) PlayerStats._nextGun = 4;
        if(Input.GetKey(keybinds[key_weapon_5])) PlayerStats._nextGun = 5;
        

        // reload
        if(Input.GetKey(keybinds[key_reload])) PlayerStats.Reload();

        // pew pew
        if(PlayerStats._currentGun == PlayerStats._nextGun && Input.GetKey(keybinds[key_shoot]) && PlayerStats.CanShoot()) {
            PlayerStats.currentGun.Shoot(rigidbody.position, mouse_offset, angle, rigidbody);
            PlayerStats.playerStats.playerHUD.UpdateAmmo();
        }

        //items
        if(PlayerStats.state == PlayerStats.PlayerState.READY && Input.GetKeyDown(keybinds[key_item])) {

            switch(PlayerStats.currentItem) {
            case Item.BLADE:
                PlayerStats.BeginMelee();
                break;
            case Item.BOMB:
                Instantiate(prefab_bomb, rigidbody.position, Quaternion.identity);
                RemoveCurrentItem();
                break;
            case Item.MEDKIT:
                PlayerStats.playerTarget.Heal(20);
                RemoveCurrentItem();
                break;
            case Item.STIMPACK:
                PlayerStats.playerTarget.Heal(10);
                RemoveCurrentItem();
                break;
            case Item.COMPASS:
                RemoveCurrentItem();
                break;
            case Item.POTION:
                RemoveCurrentItem();
                break;
            default:
                break;
            }
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
                for(int i=0; i < PlayerStats.hotbar.Length; i++) {
                    if(PlayerStats.hotbar[i] == Item.NONE) {
                        PlayerStats.hotbar[i] = PlayerStats.interactItem;
                        PlayerStats.currentItem = PlayerStats.hotbar[i];
                        PlayerStats.playerStats.playerHUD.UpdateHotbar();
                        Destroy(PlayerStats.interactPickup.gameObject);
                        PlayerStats.interactPickup = null;
                        break;
                    }
                }
            }
        }

        // drop
        if(Input.GetKeyDown(keybinds[key_drop]) && PlayerStats.currentItem != Item.NONE) {
            // Transform item = Instantiate(itemPrefabs[(int)PlayerStats.currentItem]);
            // item.position = rigidbody.position;
            // item.GetComponent<ItemPickup>().item = PlayerStats.currentItem;
            // RemoveCurrentItem();
            // item.GetComponent<Rigidbody2D>().AddForce(new Vector2((Random.value-0.5f)*100, (Random.value-0.5f)*100));
        }


        // dev hacks
        if(Input.GetKeyDown(KeyCode.Keypad4)) PlayerStats.playerTarget.Heal(1);
        if(Input.GetKeyDown(KeyCode.Keypad1)) PlayerStats.playerTarget.Damage(1);
    }



    void FixedUpdate() {
        rigidbody.AddForce(input_move*PlayerStats.k_RUN_ACCELL);
    }

}