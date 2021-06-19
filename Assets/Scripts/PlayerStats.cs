using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    // hierarchy
    public Gun[] h_guns;
    public Transform prefab_bomb;
    public Transform knifeRotationPoint;
    public Transform knifeStart;
    public LineRenderer line;

    // components
    public static Target target;
    public static PlayerHUD hud;
    public static Inventory inventory;
new public static Rigidbody2D rigidbody;
    

    // constant
    public const float k_RUN_ACCELL = 145.0f;
    public const float k_KNIFE_SPEED = 500f;
    public const float k_KNIFE_ARC = 70f;




    // upgrades + resources
    public static int coins=0;
    public static float g_KNIFE_DAMAGE=4;



    // weapons
    public static float gunTimer;
    public enum PlayerState:byte { 
        READY, RELOADING, CYCLING, MELEE
    };
    public static PlayerState state;
    private static PlayerState lastState;

    public static readonly Dictionary<Ammo, int> maxAmmo = new Dictionary<Ammo, int>{
            { Ammo.BULLETS_SMALL,   60  },
            { Ammo.BULLETS_BIG,     40  },
            { Ammo.SHELLS,          24  },
            { Ammo.ENERGY,          120 }
        };
    public static Dictionary<Ammo, int> ammo = new Dictionary<Ammo, int>{
            { Ammo.BULLETS_SMALL,   30  },
            { Ammo.BULLETS_BIG,     20   }, 
            { Ammo.SHELLS,          8   }, 
            { Ammo.ENERGY,          60  }
        };
    public static Gun[] guns;
    public static Gun currentGun;
    public static LinkedListNode<GridItem> currentGunItemNode;
    public static bool CanShoot() {
        return state == PlayerState.READY && currentGun != null && currentGun.ammo > 0;
    }


    // items
    public static Item currentItem = Item.NONE;
    public static LinkedListNode<GridItem> currentItemNode;
    public static Item interactItem = Item.NONE;
    public static ItemPickup interactPickup;
    public static int interactPriority=0;


    // other
    public static PlayerStats playerStats;
    private static sbyte knifeDirection;


    void Start() {
        inventory = GetComponent<Inventory>();
        target = GetComponent<Target>();
        hud = GetComponent<PlayerHUD>();
        rigidbody = GetComponent<Rigidbody2D>();
        playerStats = this;
        guns = h_guns;
        SwitchGun(-1);
        gunTimer = 0;
        state = PlayerState.CYCLING;
        hud.UpdateHotbar();
    }

    public static void RemoveCurrentItem() {
        Destroy(currentItemNode.Value.gameObject);
        currentItemNode.List.Remove(currentItemNode);
        currentItem = Item.NONE;
        hud.UpdateHotbar();
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
        hud.UpdateAmmo();
        AudioManager.PlayClip(target.transform.position, currentGun.audio_reload, currentGun.volume_reload, Mixer.SFX);
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

    public static void SwitchGun(sbyte _gun) {
        if(_gun == -1) {
            currentGun = null;
            playerStats.line.SetPosition(1, Vector3.zero);
            state = PlayerState.READY;
        } else {
            currentGun = guns[_gun];
            playerStats.line.SetPosition(1, Vector3.right*currentGun.range);
            state = PlayerState.CYCLING;
        }
        hud.UpdateAmmo();
        hud.UpdateHotbar();
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
            if(currentGun != null && currentGun.ammo == 0) Reload();
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
                }
            }
            break;
        }
    }
}
