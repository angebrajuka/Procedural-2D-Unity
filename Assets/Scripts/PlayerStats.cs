using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // hierarchy
    public Transform prefab_bomb;
    public Transform knifeRotationPoint;
    public Transform knifeStart;
    public Transform weapons;
    public Transform entities;

    // components
    public static PlayerStats instance;
    public static Target target;
    public static PlayerHUD hud;
    public static Inventory inventory;
new public static Rigidbody2D rigidbody;
    public static PlayerAnimator playerAnimator;
    public static PlayerInput playerInput;


    // constant
    public const float k_RUN_ACCELL = 125.0f;
    public const float k_SPRINT_MULTIPLIER = 1.2f;
    public const float k_FLASHLIGHT_MULTIPLIER = 0.88f;
    public const float k_KNIFE_SPEED = 500f;
    public const float k_KNIFE_ARC = 70f;

    // upgrades + resources
    public static float g_KNIFE_DAMAGE=4;
    public static float energyMax, energy;

    // weapons
    public static float gunRpmTimer;
    public static float gunReloadTimer;
    public static GameObject reloadSound;
    public static Gun[] guns = new Gun[9];
    public static int _currentGun;
    public static Gun currentGun;

    // items
    public static bool melee;
    private static sbyte knifeDirection;
    public static Item currentItem = Item.NONE;
    public static LinkedListNode<GridItem> currentItemNode;
    public static Item interactItem = Item.NONE;
    public static ItemPickup interactPickup;
    public static int interactPriority=0;

    //state
    public static bool sprinting=false;
    public static bool flashlight=false;


    // global
    public static byte difficulty;
    public static byte save;
    public static bool load=false;
    public static bool loadingFirstChunks=true;


    public void Init()
    {
        instance = this;
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
    }

    public void Reset()
    {
        PauseHandler.Pause();
        PauseHandler.Blur();

        SwitchGun(-1, true);
        gunRpmTimer = 0;
        gunReloadTimer = 0;
        melee = false;
        energyMax = 50;

        if(load)
        {
            Save_Load.Load(save);
        }
        else
        {
            rigidbody.position = new Vector2(325, 25);
            
            while(entities.childCount > 0)
            {
                Transform child = entities.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            DynamicEnemySpawning.Reset();

            DaylightCycle.time = DaylightCycle.k_DAY*2f/3f;

            energy = energyMax;

            inventory.Clear();

            // inventory.AutoAdd(Item.BLADE);
            // inventory.AutoAdd(Item.BOMB);
            // inventory.AutoAdd(Item.MEDKIT);
            // inventory.AutoAdd(Item.STIMPACK);
            // inventory.AutoAdd(Item.COMPASS);
            // inventory.AutoAdd(Item.POTION);
            // inventory.AutoAdd(Item.FISHING_ROD);
            // inventory.AutoAdd(Item.FLASHLIGHT);
            // inventory.AutoAdd(Item.PISTOL);
            inventory.AutoAdd(Item.SMG);
            inventory.AutoAdd(Item.ASSAULT_RIFLE);
            inventory.AutoAdd(Item.DMR);

            inventory.AutoAdd(Item.SHOTGUN_PUMP);
            // inventory.AutoAdd(Item.SHOTGUN_DOUBLE);
            // inventory.AutoAdd(Item.SHOTGUN_AUTO);
            // inventory.AutoAdd(Item.ENERGY_RIFLE);
            inventory.AutoAdd(Item.ENERGY_RAILGUN);

            inventory.AutoAdd(Item.BULLETS_SMALL, 20);
            inventory.AutoAdd(Item.BULLETS_LARGE, 20);
            inventory.AutoAdd(Item.SHELLS, 16);
            inventory.AutoAdd(Item.PLASMA, 60);

            Save_Load.Save(save);
        }
        
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
        if(inventory.isOpen || currentGun == null || GetAmmo() == currentGun.clipSize || gunReloadTimer > 0 || gunRpmTimer > 0 || inventory.GetTotalCount(currentGun.ammoType) == 0) return;
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
        int _ammo = inventory.GetTotalCount(currentGun.ammoType)/currentGun.ammoPerShot;
        int _clip = GetAmmo()/currentGun.ammoPerShot;
        int _clipSize = currentGun.clipSize/currentGun.ammoPerShot;
        
        if(_ammo > _clipSize - _clip)
        {
            inventory.RemoveItemCount(currentGun.ammoType, (_clipSize - _clip)*currentGun.ammoPerShot);
            SetAmmo(currentGun.clipSize);
        }
        else if(_ammo > 0)
        {
            int num = _ammo*currentGun.ammoPerShot;
            SetAmmo(GetAmmo()+num);
            inventory.RemoveItemCount(currentGun.ammoType, num);
        }

        hud.UpdateAmmo();
    }

    public static void BeginMelee()
    {
        CancelReload();
        melee = true;
        instance.knifeRotationPoint.gameObject.SetActive(true);
        playerAnimator.BeginMelee();
        instance.knifeStart.localEulerAngles = Vector3.forward*PlayerInput.angle;
        knifeDirection = Random.value>0.5f ? (sbyte)-1 : (sbyte)1;
        instance.knifeRotationPoint.localEulerAngles = Vector3.forward*k_KNIFE_ARC*knifeDirection;
    }

    public static void EndMelee() {
        instance.knifeRotationPoint.gameObject.SetActive(false);
        playerAnimator.EndMelee();
        melee = false;
    }

    public static void SwitchGun(sbyte _gun, bool nullNode)
    {
        CancelReload();
        
        _currentGun = _gun;
        if(_gun == -1)
        {
            currentGun = null;
        }
        else
        {
            currentGun = guns[_gun];
            currentItem = Item.NONE;
        }

        if(nullNode) currentItemNode = null;

        playerAnimator.UpdateGunImage();

        hud.UpdateAmmo();
        hud.UpdateHotbar();
    }

    public static void SetAmmo(int ammo)
    {
        if(currentItemNode != null) currentItemNode.Value.ammo = ammo;
    }

    public static int GetAmmo()
    {
        return currentItemNode == null ? 0 : currentItemNode.Value.ammo;
    }

    public static bool CanShoot()
    {
        return gunRpmTimer <= 0 && currentGun != null && GetAmmo() > 0 && gunReloadTimer <= 0;
    }

    void Update()
    {
        if(melee)
        {
            instance.knifeRotationPoint.localEulerAngles += Vector3.back*Time.deltaTime*k_KNIFE_SPEED*knifeDirection;
            
            if(instance.knifeRotationPoint.localEulerAngles.z < 360-k_KNIFE_ARC && instance.knifeRotationPoint.localEulerAngles.z > k_KNIFE_ARC)
            {
                EndMelee();
            }
        }

        if(currentGun != null && GetAmmo() == 0) BeginReload();

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
