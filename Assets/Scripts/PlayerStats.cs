using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    // hierarchy
    public Gun[] h_guns;
    public PlayerHUD playerHUD;
    public Transform knifeRotationPoint;
    public Transform knifeStart;
    

    // constant
    public const float k_RUN_ACCELL = 145.0f;
    public const float k_KNIFE_SPEED = 500f;
    public const float k_KNIFE_ARC = 70f;




    // upgrades + resources
    public static int coins=0;
    public static float g_KNIFE_DAMAGE=6;



    // primary weapons
    public static float gunTimer;
    public enum PlayerState:byte { 
        READY=0, RELOADING=1, CYCLING=2, SWITCHING=4, MELEE
    };
    public static PlayerState state;
    private static PlayerState lastState;

    public static readonly Dictionary<Ammo, int> maxAmmo = new Dictionary<Ammo, int>{
           { Ammo.BULLETS, 200 },
           { Ammo.SHELLS, 36 },
           { Ammo.ENERGY, 300 }
        };
    public static Dictionary<Ammo, int> ammo = new Dictionary<Ammo, int>(){
            {Ammo.BULLETS, 50}, 
            {Ammo.SHELLS, 12}, 
            {Ammo.ENERGY, 300}
        };
    public static Gun[] guns;
    public static Gun currentGun;
    public static int _nextGun, _currentGun;
    public static bool CanShoot() {
        return state == PlayerState.READY && currentGun.ammo > 0;
    }


    // items
    public static Item[] hotbar = new Item[12];
    public static int _item;
    public static Item currentItem = Item.BLADE;
    public static Item interactItem = Item.NONE;
    public static ItemPickup interactPickup;
    public static int interactPriority=0;


    // other
    public static PlayerStats playerStats;
    public static PlayerTarget playerTarget;
    private static sbyte knifeDirection;


    void Start() {
        playerStats = this;
        guns = h_guns;
        _nextGun = 0;
        _currentGun = 0;
        currentGun = guns[_currentGun];
        gunTimer = 0;
        state = PlayerState.SWITCHING;
        for(int i=2; i<hotbar.Length; i++) {
            hotbar[i] = Item.NONE;
        }
        hotbar[0] = Item.BLADE;
        hotbar[1] = Item.BOMB;
        playerHUD.UpdateHotbar();
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
        AudioManager.PlayClip(playerTarget.transform.position, currentGun.audio_reload, currentGun.volume_reload, Mixer.SFX);
    }

    public static void BeginMelee() {
        lastState = state;
        state = PlayerState.MELEE;
        playerStats.knifeRotationPoint.gameObject.SetActive(true);
        PlayerAnimator.playerAnimator.BeginMelee();
        playerStats.knifeStart.localEulerAngles = Vector3.forward*PlayerInput.angle;
        knifeDirection = Random.value>0.5f ? (sbyte)-1 : (sbyte)1;
        playerStats.knifeRotationPoint.localEulerAngles = Vector3.forward*k_KNIFE_ARC*knifeDirection;
    }

    void Update() {
        switch(state) {
        case PlayerState.MELEE:
            playerStats.knifeRotationPoint.localEulerAngles += Vector3.back*Time.deltaTime*k_KNIFE_SPEED*knifeDirection;
            if(playerStats.knifeRotationPoint.localEulerAngles.z < 360-k_KNIFE_ARC && playerStats.knifeRotationPoint.localEulerAngles.z > k_KNIFE_ARC) {
                // end melee

                state = lastState;
                playerStats.knifeRotationPoint.gameObject.SetActive(false);
                PlayerAnimator.playerAnimator.EndMelee();
            }
            break;
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
