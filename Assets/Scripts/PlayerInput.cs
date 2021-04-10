using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {
    
    // hierarchy
    public Transform m_items;
    public Transform knifeThrowingPoint;
    public Transform[] itemPrefabs;
    // public TilemapCollider2D decorationsLow;


    // components
    new public static Rigidbody2D rigidbody;


    // input
    private Vector2 input_move;
    private Vector2 mouse_offset;
    // output
    public static float angle;
    public static byte direction8index;



    // keybinds
    public const int key_moveEast=0, key_moveNorth=1, key_moveWest=2, key_moveSouth=3, key_shoot=4, key_slot1=5, key_slot2=6, key_slot3=7, key_slot4=8, key_slot5=9, key_slot6=10, key_reload=11, key_item=12, key_interact=13, key_drop=14;
    public static KeyCode[] keybinds = new KeyCode[15];
    
    public static void DefaultKeybinds() {
        keybinds[key_moveEast]  = KeyCode.D;
        keybinds[key_moveNorth] = KeyCode.W;
        keybinds[key_moveWest]  = KeyCode.A;
        keybinds[key_moveSouth] = KeyCode.S;
        keybinds[key_shoot]     = KeyCode.Mouse0;
        keybinds[key_slot1]     = KeyCode.Alpha1;
        keybinds[key_slot2]     = KeyCode.Alpha2;
        keybinds[key_slot3]     = KeyCode.Alpha3;
        keybinds[key_slot4]     = KeyCode.Alpha4;
        keybinds[key_slot5]     = KeyCode.Alpha5;
        keybinds[key_slot6]     = KeyCode.Alpha6;
        keybinds[key_reload]    = KeyCode.R;
        keybinds[key_item]      = KeyCode.F;
        keybinds[key_interact]  = KeyCode.E;
        keybinds[key_drop]      = KeyCode.Q;
    }

    public static void LoadKeybinds() {
        keybinds[key_moveEast]  = (KeyCode)PlayerPrefs.GetInt("key_moveEast");
        keybinds[key_moveNorth] = (KeyCode)PlayerPrefs.GetInt("key_moveNorth");
        keybinds[key_moveWest]  = (KeyCode)PlayerPrefs.GetInt("key_moveWest");
        keybinds[key_moveSouth] = (KeyCode)PlayerPrefs.GetInt("key_moveSouth");
        keybinds[key_shoot]     = (KeyCode)PlayerPrefs.GetInt("key_shoot");
    }

    public static void SaveKeybinds() {
        PlayerPrefs.SetInt("key_moveEast",  (int)keybinds[key_moveEast]);
        PlayerPrefs.SetInt("key_moveNorth", (int)keybinds[key_moveNorth]);
        PlayerPrefs.SetInt("key_moveWest",  (int)keybinds[key_moveWest]);
        PlayerPrefs.SetInt("key_moveSouth", (int)keybinds[key_moveSouth]);
        PlayerPrefs.SetInt("key_shoot",     (int)keybinds[key_shoot]);
    }



    void Start() {
        DefaultKeybinds();
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    static Vector3 halfScreen;

    void IgnoreCollisionsItem(Transform item) {
        EdgeCollider2D c = item.GetComponent<EdgeCollider2D>();
        foreach(Gun gun in PlayerStats.guns) {
            EdgeCollider2D g = gun.GetComponent<EdgeCollider2D>();
            Physics2D.IgnoreCollision(c, g);
        }
        Physics2D.IgnoreCollision(c, GetComponent<BoxCollider2D>());
    }

    void RemoveCurrentItem() {
        PlayerStats.currentItem = Item.NONE;
        PlayerStats.items[PlayerStats._item] = Item.NONE;
        PlayerStats.playerStats.playerHUD.UpdateItems();
    }

    void Update() {
        
        // mouse look
        {
            halfScreen.x = Screen.width/2;
            halfScreen.y = Screen.height/2;

            mouse_offset = Input.mousePosition-halfScreen;
            mouse_offset.Normalize();
            angle = Math.NormalizedVecToAngle(mouse_offset);

            m_items.eulerAngles = new Vector3(0, 0, angle);
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


        // switching
        if(Input.GetKey(keybinds[key_slot1])) PlayerStats._nextGun = 0;
        if(Input.GetKey(keybinds[key_slot2])) PlayerStats._nextGun = 1;
        if(Input.GetKey(keybinds[key_slot3])) PlayerStats._nextGun = 2;
        if(Input.GetKey(keybinds[key_slot4])) PlayerStats._nextGun = 3;
        if(Input.GetKey(keybinds[key_slot5])) PlayerStats._nextGun = 4;
        if(Input.GetKey(keybinds[key_slot6])) PlayerStats._nextGun = 5;
        if(Input.mouseScrollDelta.y > 0) {
            PlayerStats._nextGun --;
            if(PlayerStats._nextGun < 0) PlayerStats._nextGun = 5;
        } else if(Input.mouseScrollDelta.y < 0) {
            PlayerStats._nextGun ++;
            if(PlayerStats._nextGun > 5) PlayerStats._nextGun = 0;
        }
        

        // reload
        if(Input.GetKey(keybinds[key_reload])) PlayerStats.Reload();

        // pew pew
        if(PlayerStats._currentGun == PlayerStats._nextGun && Input.GetKey(keybinds[key_shoot]) && PlayerStats.CanShoot()) {
            PlayerStats.currentGun.Shoot(rigidbody.position, mouse_offset, angle, rigidbody);
            PlayerStats.playerStats.playerHUD.UpdateAmmo();
        }

        //items
        if(Input.GetKeyDown(keybinds[key_item])) {

            switch(PlayerStats.currentItem) {
            case Item.BLADE:
                Transform knife = Instantiate(itemPrefabs[(int)Item.BLADE], knifeThrowingPoint.position, Quaternion.identity);
                Rigidbody2D rb = knife.GetComponent<Rigidbody2D>();
                rb.AddForce(mouse_offset*900);
                rb.AddForce(rigidbody.velocity*50);
                rb.AddTorque(1200);
                IgnoreCollisionsItem(knife);

                break;
            case Item.BOMB:

                break;
            case Item.MEDKIT:
                PlayerStats.player.Heal(20);
                break;
            case Item.STIMPACK:
                PlayerStats.player.Heal(10);
                break;
            case Item.COMPASS:
                
                break;
            case Item.POTION:

                break;
            default:
                break;
            }

            RemoveCurrentItem();
        }


        // interact
        if(Input.GetKeyDown(keybinds[key_interact])) {
            if(PlayerStats.interactItem != Item.NONE) {
                for(int i=0; i < PlayerStats.items.Length; i++) {
                    if(PlayerStats.items[i] == Item.NONE) {
                        PlayerStats.items[i] = PlayerStats.interactItem;
                        PlayerStats.playerStats.playerHUD.UpdateItems();
                        Destroy(PlayerStats.interactPickup.gameObject);
                        PlayerStats.interactPickup = null;
                        break;
                    }
                }
            }
        }

        // drop
        if(Input.GetKeyDown(keybinds[key_drop]) && PlayerStats.currentItem != Item.NONE) {
            Transform item = Instantiate(itemPrefabs[(int)PlayerStats.currentItem]);
            item.position = rigidbody.position;
            item.GetComponent<ItemPickup>().item = PlayerStats.currentItem;
            RemoveCurrentItem();
            item.GetComponent<Rigidbody2D>().AddForce(new Vector2((Random.value-0.5f)*100, (Random.value-0.5f)*100));
            IgnoreCollisionsItem(item);
        }


        // dev hacks
        if(Input.GetKeyDown(KeyCode.Keypad4)) PlayerStats.player.Heal(1);
        if(Input.GetKeyDown(KeyCode.Keypad1)) PlayerStats.player.Damage(1);
    }



    void FixedUpdate() {
        rigidbody.AddForce(input_move*PlayerStats.k_RUN_ACCELL);
    }

}