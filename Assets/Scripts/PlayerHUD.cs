using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    public Text bulletsTxt, shellsTxt, energyTxt;
    public Text currentGunAmmoTxt;
    public Text currentReserveTxt;
    public Text interact;
    public Text textCoins;
    public Image hotbarPrimary, hotbarSecondary;
    public RectTransform ammoTxt;
    public Sprite[] sprite_items;

    public Slider bar_health;

    void Start() {
        UpdateAmmo();
        UpdateHealth();
    }

    // static readonly Vector2[] positionsItems = {    new Vector2(0, 0),
    //                                                 new Vector2(50, 0),
    //                                                 new Vector2(100, 0),
    //                                                 new Vector2(150, 0),
    //                                                 new Vector2(200, 0),
    //                                                 new Vector2(250, 0) };

    public void UpdateHotbar() {
        hotbarPrimary.sprite    = PlayerStats.currentGun.sprite;
        hotbarSecondary.sprite  = sprite_items[(int)PlayerStats.currentItem];
    }

    public void UpdateAmmo() {
        bulletsTxt.text = PlayerStats.ammo[Ammo.BULLETS]+"";
        shellsTxt.text = PlayerStats.ammo[Ammo.SHELLS]+"";
        energyTxt.text = PlayerStats.ammo[Ammo.ENERGY]+"";
        currentGunAmmoTxt.text = PlayerStats.currentGun.ammo+"";///"+PlayerStats.ammo[PlayerStats.currentGun.ammoType];
        currentReserveTxt.text = "/"+PlayerStats.ammo[PlayerStats.currentGun.ammoType];
    }

    public void UpdateHealth() {
        bar_health.value = PlayerStats.playerTarget.health / PlayerStats.playerTarget.maxHealth;
    }

    public void UpdateCoins() {
        textCoins.text = ""+PlayerStats.coins;
    }
}
