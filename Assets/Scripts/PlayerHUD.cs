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
    public Image[] inventoryCells;
    public RectTransform highlight_item;
    public RectTransform ammoTxt;
    public Sprite[] sprite_items;

    public Slider bar_health;

    void Start() {
        UpdateAmmo();
        UpdateHealth();
    }

    static readonly Vector2[] positionsItems = {    new Vector2(0, 0),
                                                    new Vector2(50, 0),
                                                    new Vector2(100, 0),
                                                    new Vector2(150, 0),
                                                    new Vector2(200, 0),
                                                    new Vector2(250, 0) };
    public void UpdateHotbar() {
        for(int i=0; i<PlayerStats.hotbar.Length; i++) {
            inventoryCells[i].sprite = sprite_items[(int)PlayerStats.hotbar[i]];
        }
        highlight_item.anchoredPosition = positionsItems[PlayerStats._item];
        PlayerStats.currentItem = PlayerStats.hotbar[PlayerStats._item];
    }

    public void UpdateAmmo() {
        bulletsTxt.text = PlayerStats.ammo[Ammo.BULLETS]+"";
        shellsTxt.text = PlayerStats.ammo[Ammo.SHELLS]+"";
        energyTxt.text = PlayerStats.ammo[Ammo.ENERGY]+"";
        currentGunAmmoTxt.text = PlayerStats.currentGun.ammo+"";
        currentReserveTxt.text = PlayerStats.ammo[PlayerStats.currentGun.ammoType]+"";
    }

    public void UpdateHealth() {
        bar_health.value = PlayerStats.playerTarget.health / PlayerStats.playerTarget.maxHealth;
    }

    public void UpdateCoins() {
        int numOfDigits = (PlayerStats.coins+"").Length;
        textCoins.text = new string('0', 11-numOfDigits) + PlayerStats.coins;
    }
}
