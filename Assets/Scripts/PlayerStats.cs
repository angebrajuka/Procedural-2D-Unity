using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // hierarchy
    public Transform prefab_bomb;
    public Transform knifeRotationPoint;
    public Transform knifeStart;
    public LineRenderer line;
    public Transform weapons;

    // components
    public static Target target;
    public static PlayerHUD hud;
    public static Inventory inventory;
new public static Rigidbody2D rigidbody;
    public static PlayerAnimator playerAnimator;
    public static PlayerInput playerInput;
    

    // constant
    public const float k_RUN_ACCELL = 145.0f;
    public const float k_KNIFE_SPEED = 500f;
    public const float k_KNIFE_ARC = 70f;




    // upgrades + resources
    public static int coins=0;
    public static float g_KNIFE_DAMAGE=4;



    // weapons
    public static float gunRpmTimer;
    public static float gunReloadTimer;
    public static GameObject reloadSound;

    public static readonly Dictionary<Ammo, int> maxAmmo = new Dictionary<Ammo, int>
    {
        { Ammo.BULLETS_SMALL,   60  },
        { Ammo.BULLETS_BIG,     40  },
        { Ammo.SHELLS,          24  },
        { Ammo.ENERGY,          120 }
    };
    public static Dictionary<Ammo, int> ammo = new Dictionary<Ammo, int>
    {
        { Ammo.BULLETS_SMALL,   30  },
        { Ammo.BULLETS_BIG,     20   }, 
        { Ammo.SHELLS,          8   }, 
        { Ammo.ENERGY,          60  }
    };
    public static Gun[] guns = new Gun[9];
    public static Gun currentGun;
    public static LinkedListNode<GridItem> currentGunItemNode;
    


    public static bool CanShoot() { return gunRpmTimer <= 0 && currentGun != null && currentGun.ammo > 0 && gunReloadTimer <= 0; }



    // items
    public static bool melee;
    public static Item currentItem = Item.NONE;
    public static LinkedListNode<GridItem> currentItemNode;
    public static Item interactItem = Item.NONE;
    public static ItemPickup interactPickup;
    public static int interactPriority=0;


    // other
    public static PlayerStats playerStats;
    private static sbyte knifeDirection;


    void Start()
    {
        playerStats = this;
        inventory = GetComponent<Inventory>();
        target = GetComponent<Target>();
        hud = GetComponent<PlayerHUD>();
        rigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerInput = GetComponent<PlayerInput>();

        for(int i=0; i<guns.Length; i++)
        {
            guns[i] = weapons.GetChild(i).GetComponent<Gun>();
        }

        Inventory.items.Clear();


        inventory.AutoAdd(Item.BLADE);
        inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.MEDKIT);
        // inventory.AutoAdd(Item.STIMPACK);
        // inventory.AutoAdd(Item.COMPASS);
        // inventory.AutoAdd(Item.POTION);
        inventory.AutoAdd(Item.FISHING_ROD);
        inventory.AutoAdd(Item.FLASHLIGHT);
        inventory.AutoAdd(Item.PISTOL);
        inventory.AutoAdd(Item.SMG);
        inventory.AutoAdd(Item.ASSAULT_RIFLE);
        inventory.AutoAdd(Item.DMR);
        inventory.AutoAdd(Item.SHOTGUN_PUMP);
        inventory.AutoAdd(Item.SHOTGUN_DOUBLE);
        inventory.AutoAdd(Item.SHOTGUN_AUTO);
        inventory.AutoAdd(Item.ENERGY_RIFLE);
        inventory.AutoAdd(Item.ENERGY_RAILGUN);

        // inventory.AutoAdd(Item.PISTOL);
        // inventory.AutoAdd(Item.SMG);
        // inventory.AutoAdd(Item.ASSAULT_RIFLE);
        // inventory.AutoAdd(Item.DMR);
        // inventory.AutoAdd(Item.SHOTGUN_PUMP);
        // inventory.AutoAdd(Item.SHOTGUN_DOUBLE);
        // inventory.AutoAdd(Item.SHOTGUN_AUTO);
        // inventory.AutoAdd(Item.FLASHLIGHT);
        // inventory.AutoAdd(Item.ENERGY_RAILGUN);
        // inventory.AutoAdd(Item.SMG);
        // inventory.AutoAdd(Item.PISTOL);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);
        // inventory.AutoAdd(Item.BOMB);

        SwitchGun(-1);
        gunRpmTimer = 0;
        gunReloadTimer = 0;
        melee = false;
        hud.UpdateHotbar();
    }

    public static void RemoveCurrentItem()
    {
        Destroy(currentItemNode.Value.gameObject);
        currentItemNode.List.Remove(currentItemNode);
        currentItem = Item.NONE;
        hud.UpdateHotbar();
    }

    public static void BeginReload()
    {
        if(Inventory.isOpen || currentGun == null || currentGun.ammo == currentGun.clipSize || gunReloadTimer > 0 || gunRpmTimer > 0 || ammo[currentGun.ammoType] == 0) return;
        gunReloadTimer = currentGun.reloadTime;
        reloadSound = AudioManager.PlayClip(currentGun.audio_reload, currentGun.volume_reload, Mixer.SFX);
    }

    public static void CancelReload()
    {
        gunReloadTimer = 0;
        Destroy(reloadSound);
    }

    public static void FinishReload()
    {
        int _ammo = ammo[currentGun.ammoType]/currentGun.ammoPerShot;
        int _clip = currentGun.ammo/currentGun.ammoPerShot;
        int _clipSize = currentGun.clipSize/currentGun.ammoPerShot;
        
        if(_ammo > _clipSize - _clip)
        {
            ammo[currentGun.ammoType] -= (_clipSize - _clip)*currentGun.ammoPerShot;
            currentGun.ammo = currentGun.clipSize;
        }
        else if(_ammo > 0)
        {
            int num = _ammo*currentGun.ammoPerShot;
            currentGun.ammo += num;
            ammo[currentGun.ammoType] -= num;
        }

        hud.UpdateAmmo();
    }

    public static void BeginMelee()
    {
        CancelReload();
        melee = true;
        playerStats.knifeRotationPoint.gameObject.SetActive(true);
        PlayerAnimator.playerAnimator.BeginMelee();
        playerStats.knifeStart.localEulerAngles = Vector3.forward*PlayerInput.angle;
        knifeDirection = Random.value>0.5f ? (sbyte)-1 : (sbyte)1;
        playerStats.knifeRotationPoint.localEulerAngles = Vector3.forward*k_KNIFE_ARC*knifeDirection;
    }

    public static void SwitchGun(sbyte _gun)
    {
        CancelReload();
        
        if(_gun == -1)
        {
            currentGun = null;
            playerStats.line.SetPosition(1, Vector3.zero);
        }
        else
        {
            currentGun = guns[_gun];
            playerStats.line.SetPosition(1, Vector3.right*currentGun.range);
        }

        playerAnimator.UpdateGunImage();

        hud.UpdateAmmo();
        hud.UpdateHotbar();
    }

    void Update()
    {
        if(melee)
        {
            playerStats.knifeRotationPoint.localEulerAngles += Vector3.back*Time.deltaTime*k_KNIFE_SPEED*knifeDirection;
            
            if(playerStats.knifeRotationPoint.localEulerAngles.z < 360-k_KNIFE_ARC && playerStats.knifeRotationPoint.localEulerAngles.z > k_KNIFE_ARC)
            {
                // end melee
                playerStats.knifeRotationPoint.gameObject.SetActive(false);
                PlayerAnimator.playerAnimator.EndMelee();
            }
        }

        if(currentGun != null && currentGun.ammo == 0) BeginReload();

        if(gunReloadTimer > 0)
        {
            gunReloadTimer -= Time.deltaTime;
            if(gunReloadTimer <= 0) FinishReload();
        }

        if(gunRpmTimer > 0)
        {
            gunRpmTimer -= Time.deltaTime;
        }
    }
}
