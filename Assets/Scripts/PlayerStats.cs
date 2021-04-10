using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    // hierarchy
    public Gun[] h_guns;
    public PlayerHUD playerHUD;
    

    // constant
    public const float k_RUN_ACCELL = 70.0f;



    // upgrades + resources
    public static int coins=0;



    // primary weapons
    public static float gunTimer;
    public enum PlayerState:byte { READY=0, RELOADING=1, CYCLING=2, SWITCHING=4 }; public static PlayerState state;
    public static readonly Dictionary<Ammo, int> maxAmmo = new Dictionary<Ammo, int>{
           { Ammo.BULLETS, 200 },
           { Ammo.SHELLS, 50 },
           { Ammo.ENERGY, 300 }
        };
    public static Dictionary<Ammo, int> ammo = new Dictionary<Ammo, int>(){
            {Ammo.BULLETS, 50}, 
            {Ammo.SHELLS, 20}, 
            {Ammo.ENERGY, 300}
        };
    public static Gun[] guns;
    public static Gun currentGun;
    public static int _nextGun, _currentGun;
    public static bool CanShoot() {
        return state == PlayerState.READY && currentGun.ammo > 0;
    }


    // items
    public static Item[] items = {Item.BLADE, Item.NONE, Item.NONE, Item.NONE, Item.NONE, Item.NONE, Item.NONE, Item.NONE};
    public static int _item;
    public static Item currentItem = Item.NONE;
    public static Item interactItem = Item.NONE;
    public static ItemPickup interactPickup;
    public static int interactPriority=0;


    // other
    public static PlayerStats playerStats;
    public static Player player;


    void Start() {
        playerStats = this;
        guns = h_guns;
        _nextGun = 0;
        _currentGun = 0;
        currentGun = guns[_currentGun];
        gunTimer = 0;
        state = PlayerState.SWITCHING;
        playerHUD.UpdateItems();
    }

    public static void Reload() {
        if(currentGun.ammo == currentGun.clipSize || state != PlayerState.READY) return;
        int _ammo = ammo[currentGun.ammoType]/currentGun.ammoPerShot;
        int _clip = currentGun.ammo/currentGun.ammoPerShot;
        int _clipSize = currentGun.clipSize/currentGun.ammoPerShot;
        if(_ammo > _clipSize - _clip) {
            ammo[currentGun.ammoType] -= (_clipSize - _clip)*currentGun.ammoPerShot;
            currentGun.ammo = currentGun.clipSize;
        } else if(_ammo > 0) {
            int num = _ammo*currentGun.ammoPerShot;
            currentGun.ammo += num;
            ammo[currentGun.ammoType] -= num;
        } else {
            state = PlayerState.READY;
            return;
        }

        state = PlayerState.RELOADING;
        gunTimer = currentGun.reloadTime;
        playerStats.playerHUD.UpdateAmmo();
        AudioManager.PlayClip(player.transform.position, currentGun.audio_reload, currentGun.volume_reload, Mixer.SFX);
    }

    void Update() {
        switch(state) {
        case PlayerState.READY:
            if(_nextGun != _currentGun) state = PlayerState.SWITCHING;
            else if(currentGun.ammo == 0) Reload();
            break;
        default:
            gunTimer -= Time.deltaTime;
            if(gunTimer <= 0) {
                switch(state) {
                case PlayerState.RELOADING:
                    state = PlayerState.READY;
                    break;
                case PlayerState.CYCLING:
                    state = PlayerState.READY;
                    break;
                case PlayerState.SWITCHING:
                    _currentGun = _nextGun;
                    currentGun = guns[_currentGun];
                    state = PlayerState.CYCLING;
                    playerHUD.UpdateAmmo();
                    playerHUD.UpdateHotbar();
                    break;
                }
            }
            break;
        }
    }
}
