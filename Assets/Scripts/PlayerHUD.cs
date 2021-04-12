using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    public Text bulletsTxt, shellsTxt, energyTxt;
    public Text currentGunAmmoTxt;
    public Text interact;
    public Text textCoins;
    public Image[] inventoryCells;
    public RectTransform highlight_gun, highlight_item;
    public RectTransform ammoTxt;
    public Sprite[] sprite_items;

    public Slider bar_health;

    void Start() {
        UpdateAmmo();
        UpdateHealth();
    }

    // static readonly Vector2[] positionsHotbar = {   new Vector2(0, 125),
    //                                                 new Vector2(0, 75),
    //                                                 new Vector2(0, 25),
    //                                                 new Vector2(0, -25),
    //                                                 new Vector2(0, -75),
    //                                                 new Vector2(0, -125),
    //                                                 new Vector2(0, -175),
    //                                                 new Vector2(0, -225) };
    // public void UpdateHotbar() {
    //     highlight_gun.anchoredPosition = positionsHotbar[PlayerStats._currentGun];
    // }

    static readonly Vector2[] positionsItems = {    new Vector2(-25, 75),
                                                    new Vector2(25, 75),
                                                    new Vector2(-25, 25),
                                                    new Vector2(25, 25),
                                                    new Vector2(-25, -25),
                                                    new Vector2(25, -25),
                                                    new Vector2(-25, -75),
                                                    new Vector2(25, -75) };
    public void UpdateItems() {
        for(int i=0; i<PlayerStats.items.Length; i++) {
            inventoryCells[i].sprite = sprite_items[(int)PlayerStats.items[i]];
        }
        highlight_item.anchoredPosition = positionsItems[PlayerStats._item];
        PlayerStats.currentItem = PlayerStats.items[PlayerStats._item];
    }

    public readonly Vector2[] positionsAmmo = { new Vector2(-71, 33),
                                                new Vector2(-71, 0),
                                                new Vector2(-71, -33) };
    public void UpdateAmmo() {
        bulletsTxt.text = PlayerStats.ammo[Ammo.BULLETS]+"";
        shellsTxt.text = PlayerStats.ammo[Ammo.SHELLS]+"";
        energyTxt.text = PlayerStats.ammo[Ammo.ENERGY]+"";
        currentGunAmmoTxt.text = PlayerStats.currentGun.ammo+"";
        ammoTxt.anchoredPosition = positionsAmmo[(int)PlayerStats.currentGun.ammoType];
    }

    public void UpdateHealth() {
        bar_health.value = PlayerStats.playerTarget.health / PlayerStats.playerTarget.maxHealth;
    }

    public void UpdateCoins() {
        int numOfDigits = (PlayerStats.coins+"").Length;
        textCoins.text = new string('0', 11-numOfDigits) + PlayerStats.coins;
    }
}
