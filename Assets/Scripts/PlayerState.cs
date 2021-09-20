using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStats;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance;

    // hierarchy
    public Transform knifeRotationPoint;
    public Transform knifeStart;

    // general
    public static bool moving=false;
    public static bool sprinting=false;
    public static bool shooting=false;
    public static bool punching=false;

    // weapons
    public static float gunRpmTimer;
    public static float gunReloadTimer;
    public static GameObject reloadSound;

    // items
    public static bool melee;
    public static sbyte knifeDirection;
    public static LinkedListNode<GridItem> currentItemNode;
    public static string _currentItem
    {
        get
        {
            return currentItemNode == null ? null : currentItemNode.Value.item.name;
        }
    }
    public static ItemStats currentItem
    {
        get
        {
            return currentItemNode == null ? null : currentItemNode.Value.item;
        }
    }
    public static string interactItem = null;
    public static ItemPickup interactPickup;

    public void Init()
    {
        instance = this;
    }

    public void Reset()
    {
        PauseHandler.Pause();
        PauseHandler.Blur();

        PlayerState.SwitchGun(true);
        gunRpmTimer = 0;
        gunReloadTimer = 0;
        melee = false;
        energyMax = 50;
        ProceduralGeneration.reset = true;

        Entities.Clear();

        if(load)
        {
            Save_Load.Load(save);
        }
        else
        {
            PlayerMovement.rb.position = ProceduralGeneration.center;
            transform.position = PlayerMovement.rb.position;

            DynamicEnemySpawning.Reset();

            DaylightCycle.time = DaylightCycle.k_DAY*2f/3f;

            energy = energyMax;

            Inventory.instance.Clear();

            difficulty = 2;

            Save_Load.Save(save);
        }
        
        PlayerHUD.instance.UpdateHotbar();
    }

    public static bool CanShoot()
    {
        return currentItem != null && currentItem.gun != null && GetAmmo() > 0 && gunReloadTimer <= 0;
    }

    public static void BeginReload()
    {
        if(Inventory.instance.isOpen || currentItem == null || currentItem.gun == null || GetAmmo() == currentItem.gun.clipSize || gunReloadTimer > 0 || gunRpmTimer > 0 || Inventory.instance.GetTotalCount(currentItem.gun.ammoType) == 0) return;
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
        int _ammo = Inventory.instance.GetTotalCount(currentItem.gun.ammoType)/currentItem.gun.ammoPerShot;
        int _clip = GetAmmo()/currentItem.gun.ammoPerShot;
        int _clipSize = currentItem.gun.clipSize/currentItem.gun.ammoPerShot;
        
        if(_ammo > _clipSize - _clip)
        {
            Inventory.instance.RemoveItemCount(currentItem.gun.ammoType, (_clipSize - _clip)*currentItem.gun.ammoPerShot);
            SetAmmo(currentItem.gun.clipSize);
        }
        else if(_ammo > 0)
        {
            int num = _ammo*currentItem.gun.ammoPerShot;
            SetAmmo(GetAmmo()+num);
            Inventory.instance.RemoveItemCount(currentItem.gun.ammoType, num);
        }

        PlayerHUD.instance.UpdateAmmo();
    }

    public static void BeginMelee()
    {
        CancelReload();
        melee = true;
        instance.knifeRotationPoint.gameObject.SetActive(true);
        PlayerAnimator.instance.BeginMelee();
        instance.knifeStart.localEulerAngles = Vector3.forward*PlayerInput.angle;
        knifeDirection = Random.value>0.5f ? (sbyte)-1 : (sbyte)1;
        instance.knifeRotationPoint.localEulerAngles = Vector3.forward*k_KNIFE_ARC*knifeDirection;
    }

    public static void EndMelee()
    {
        instance.knifeRotationPoint.gameObject.SetActive(false);
        PlayerAnimator.instance.EndMelee();
        melee = false;
    }

    public static void SwitchGun(bool nullNode)
    {
        CancelReload();

        if(nullNode) currentItemNode = null;

        PlayerHUD.instance.UpdateAmmo();
        PlayerHUD.instance.UpdateHotbar();
    }

    void Update()
    {
        if(melee)
        {
            knifeRotationPoint.localEulerAngles += Vector3.back*Time.deltaTime*k_KNIFE_SPEED*knifeDirection;
            
            if(knifeRotationPoint.localEulerAngles.z < 360-k_KNIFE_ARC && knifeRotationPoint.localEulerAngles.z > k_KNIFE_ARC)
            {
                EndMelee();
            }
        }

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