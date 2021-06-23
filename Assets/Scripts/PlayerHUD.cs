using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    // hierarchy
new public Transform transform;
    public Text currentGunAmmoTxt;
    public Text currentReserveTxt;
    public Text interact;
    public Text textCoins;
    public Image ammoImage;
    public Image currentItem;
    public RectTransform ammoTxt;
    public Sprite[] ammoImages;
    public Text smallBulletsTxt, bigBulletsTxt, shellsTxt, energyTxt;
    public Slider bar_health;
    public Image bar_health_fillRect;

    // components
    Target m_target;

    void Start() {
        m_target = GetComponent<Target>();
        bar_health_fillRect = bar_health.fillRect.GetComponent<Image>();
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
        smallBulletsTxt.text = PlayerStats.ammo[Ammo.BULLETS_SMALL]+"";
        bigBulletsTxt.text = PlayerStats.ammo[Ammo.BULLETS_BIG]+"";
        shellsTxt.text = PlayerStats.ammo[Ammo.SHELLS]+"";
        energyTxt.text = PlayerStats.ammo[Ammo.ENERGY]+"";
        if(PlayerStats.currentGun == null) {

        } else {
            currentGunAmmoTxt.text = PlayerStats.currentGun.ammo < 10 ? "0" : "";
            currentGunAmmoTxt.text += PlayerStats.currentGun.ammo+"";
            currentReserveTxt.text = "/"+PlayerStats.ammo[PlayerStats.currentGun.ammoType];
        }
    }

    public void UpdateHealth() {
        bar_health.value = m_target.health / m_target.maxHealth;
        // bar_health_fillRect.color = new Color32((byte)(bar_health.value > 0.5f ? (1-bar_health.value)*510 : 255), (byte)(bar_health.value < 0.5f ? (bar_health.value)*510 : 255), 0, 100);
    }

    public void UpdateCoins() {
        textCoins.text = ""+PlayerStats.coins;
    }
}
