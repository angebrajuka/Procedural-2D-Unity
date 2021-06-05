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
    public Image ammoImage;
    public Image[] hotbar;
    public RectTransform itemHighlight;
    public RectTransform ammoTxt;
    public Sprite[] sprite_items;
    public Sprite[] ammoImages;

    public Slider bar_health;

    void Start() {
        UpdateAmmo();
        UpdateHealth();
    }

    public void UpdateHotbar() {
        ammoImage.sprite = ammoImages[(int)PlayerStats.currentGun.ammoType];
        for(int i=0; i<PlayerStats.hotbar.Length; i++) {
            hotbar[i].sprite = sprite_items[(int)PlayerStats.hotbar[i]];
        }
        itemHighlight.anchoredPosition = new Vector3(0, -50*PlayerStats._item, 0);
    }

    public void UpdateAmmo() {
        bulletsTxt.text = PlayerStats.ammo[Ammo.BULLETS]+"";
        shellsTxt.text = PlayerStats.ammo[Ammo.SHELLS]+"";
        energyTxt.text = PlayerStats.ammo[Ammo.ENERGY]+"";
        currentGunAmmoTxt.text = PlayerStats.currentGun.ammo < 10 ? "0" : "";
        currentGunAmmoTxt.text += PlayerStats.currentGun.ammo+"";
        currentReserveTxt.text = "/"+PlayerStats.ammo[PlayerStats.currentGun.ammoType];
    }

    public void UpdateHealth() {
        bar_health.value = PlayerStats.playerTarget.health / PlayerStats.playerTarget.maxHealth;
    }

    public void UpdateCoins() {
        textCoins.text = ""+PlayerStats.coins;
    }
}
