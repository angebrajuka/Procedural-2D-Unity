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
    public float debugSpode;
    public Transform gunSpriteTransform;

    // components
    public static PlayerStats instance;
    public static Target target;
    public static PlayerHUD hud;
    public static Inventory inventory;
new public static Rigidbody2D rigidbody;
    public static PlayerAnimator playerAnimator;
    public static PlayerInput playerInput;
new public static Collider2D collider;


    // constant
    public const float k_RUN_ACCELL = 90.0f;
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

    // items
    public static bool melee;
    private static sbyte knifeDirection;
    public static string _currentItem = null;
    public static ItemStats currentItem = null;
    public static LinkedListNode<GridItem> currentItemNode;
    public static string interactItem = null;
    public static ItemPickup interactPickup;
    public static int interactPriority=0;

    //state
    public static bool sprinting=false;
    public static bool flashlight=false;


    // global
    public static byte difficulty; // 0 to 4
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
        collider = GetComponent<BoxCollider2D>();
    }

    public void Reset()
    {
        PauseHandler.Pause();
        PauseHandler.Blur();

        SwitchGun("", true);
        gunRpmTimer = 0;
        gunReloadTimer = 0;
        melee = false;
        energyMax = 50;
        ProceduralGeneration.reset = true;

        if(load)
        {
            Save_Load.Load(save);
        }
        else
        {
            rigidbody.position = ProceduralGeneration.center;
            
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

            difficulty = 2;

            Save_Load.Save(save);
        }
        
        hud.UpdateHotbar();
    }

    public static void SubtractCurrentItem()
    {
        currentItemNode.Value.count --;
        if(currentItemNode.Value.count <= 0)
        {
            RemoveCurrentItem();
        } else
        {
            hud.UpdateHotbar();
        }
    }

    public static void RemoveCurrentItem()
    {
        Destroy(currentItemNode.Value.gameObject);
        currentItemNode.List.Remove(currentItemNode);
        currentItem = null;
        hud.UpdateHotbar();
    }

    public static void BeginReload()
    {
        if(inventory.isOpen || currentItem == null || currentItem.gun == null || GetAmmo() == currentItem.gun.clipSize || gunReloadTimer > 0 || gunRpmTimer > 0 || inventory.GetTotalCount(currentItem.gun.ammoType) == 0) return;
        gunReloadTimer = currentItem.gun.reloadTime;
        reloadSound = AudioManager.PlayClip(currentItem.gun.audio_reload, currentItem.gun.volume_reload, Mixer.SFX);
    }

    public static void CancelReload()
    {
        gunReloadTimer = 0;
        Destroy(reloadSound);
    }

    public static void FinishReload()
    {
        int _ammo = inventory.GetTotalCount(currentItem.gun.ammoType)/currentItem.gun.ammoPerShot;
        int _clip = GetAmmo()/currentItem.gun.ammoPerShot;
        int _clipSize = currentItem.gun.clipSize/currentItem.gun.ammoPerShot;
        
        if(_ammo > _clipSize - _clip)
        {
            inventory.RemoveItemCount(currentItem.gun.ammoType, (_clipSize - _clip)*currentItem.gun.ammoPerShot);
            SetAmmo(currentItem.gun.clipSize);
        }
        else if(_ammo > 0)
        {
            int num = _ammo*currentItem.gun.ammoPerShot;
            SetAmmo(GetAmmo()+num);
            inventory.RemoveItemCount(currentItem.gun.ammoType, num);
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

    public static void SwitchGun(string _gun, bool nullNode)
    {
        CancelReload();
        
        if(Items.guns.ContainsKey(_gun))
        {
            _currentItem = _gun;
            currentItem = Items.items[_gun];
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
        return currentItem != null && currentItem.gun != null && GetAmmo() > 0 && gunReloadTimer <= 0;
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

        if(currentItem != null && currentItem.gun != null && GetAmmo() == 0) BeginReload();

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
