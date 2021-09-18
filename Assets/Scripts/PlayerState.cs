using UnityEngine;
using static PlayerStats;

public class PlayerState : MonoBehaviour
{
    public static bool moving=false;
    public static bool sprinting=false;
    public static bool shooting=false;

    public void Init()
    {

    }

    public static bool CanShoot()
    {
        return currentItem != null && currentItem.gun != null && GetAmmo() > 0 && gunReloadTimer <= 0;
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