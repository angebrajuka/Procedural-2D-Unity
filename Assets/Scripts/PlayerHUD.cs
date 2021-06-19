using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    // hierarchy
    public Text currentGunAmmoTxt;
    public Text currentReserveTxt;
    public Text interact;
    public Text textCoins;
    public Image ammoImage;
    public Image currentItem;
    public RectTransform itemHighlight;
    public RectTransform ammoTxt;
    public Sprite[] ammoImages;
    // public Text bulletsTxt, shellsTxt, energyTxt;
    public Slider bar_health;

    // components
    Target m_target;

    void Start() {
        m_target = GetComponent<Target>();
        UpdateAmmo();
        UpdateHealth();
    }

    public void UpdateHotbar() {
        if(PlayerStats.currentGun == null) {
            ammoTxt.gameObject.SetActive(false);
        } else {
            ammoTxt.gameObject.SetActive(true);
            ammoImage.sprite = ammoImages[(int)PlayerStats.currentGun.ammoType];
        }

        currentItem.sprite = Items.items[(int)PlayerStats.currentItem].sprite;
    }

    public void UpdateAmmo() {
        // bulletsTxt.text = PlayerStats.ammo[Ammo.BULLETS]+"";
        // shellsTxt.text = PlayerStats.ammo[Ammo.SHELLS]+"";
        // energyTxt.text = PlayerStats.ammo[Ammo.ENERGY]+"";
        if(PlayerStats.currentGun == null) {

        } else {
            currentGunAmmoTxt.text = PlayerStats.currentGun.ammo < 10 ? "0" : "";
            currentGunAmmoTxt.text += PlayerStats.currentGun.ammo+"";
            currentReserveTxt.text = "/"+PlayerStats.ammo[PlayerStats.currentGun.ammoType];
        }
    }

    public void UpdateHealth() {
        bar_health.value = m_target.health / m_target.maxHealth;
    }

    public void UpdateCoins() {
        textCoins.text = ""+PlayerStats.coins;
    }
}
