using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    // hierarchy
new public Transform transform;
    public Text currentGunAmmoTxt;
    public Text currentReserveTxt;
    public Text interact;
    public Image ammoImage;
    public Image currentItem;
    public RectTransform ammoTxt;
    public Sprite[] ammoImages;
    // public Text smallBulletsTxt, bigBulletsTxt, shellsTxt, energyTxt;
    public Slider bar_health;
    public Image bar_health_fillRect;
    public Slider bar_energy;
    public RectTransform minimap;
    public RectTransform minimapPlayer;

    // components
    Target m_target;

    void Start()
    {
        m_target = GetComponent<Target>();
        bar_health_fillRect = bar_health.fillRect.GetComponent<Image>();
        UpdateAmmo();
        UpdateHealth();
    }

    public void UpdateHotbar()
    {
        if(PlayerStats.currentGun == null)
        {
            ammoTxt.gameObject.SetActive(false);
        }
        else
        {
            ammoTxt.gameObject.SetActive(true);
            ammoImage.sprite = ammoImages[(int)PlayerStats.currentGun.ammoType-(int)Item.BULLETS_SMALL];
        }

        ItemStats item = Items.items[(int)PlayerStats.currentItem];
        currentItem.sprite = item.sprite;
        currentItem.transform.localScale = PlayerStats.currentItem == Item.NONE ? Vector3.zero : new Vector3(item.size.x > item.size.y ? 1 : (float)item.size.x/item.size.y, item.size.y > item.size.x ? 1 : (float)item.size.y/item.size.x, 1);
        currentItem.transform.parent.gameObject.SetActive(PlayerStats.currentItem != Item.NONE);
    }

    public void UpdateAmmo()
    {
        // smallBulletsTxt.text = PlayerStats.inventory.GetTotalCount(Item.BULLETS_SMALL)+"";
        // bigBulletsTxt.text = PlayerStats.inventory.GetTotalCount(Item.BULLETS_LARGE)+"";
        // shellsTxt.text = PlayerStats.inventory.GetTotalCount(Item.SHELLS)+"";
        // energyTxt.text = PlayerStats.inventory.GetTotalCount(Item.PLASMA)+"";
        if(PlayerStats.currentGun != null)
        {
            currentGunAmmoTxt.text = PlayerStats.currentGun.ammo < 10 ? "0" : "";
            currentGunAmmoTxt.text += PlayerStats.currentGun.ammo+"";
            currentReserveTxt.text = "/"+PlayerStats.inventory.GetTotalCount(PlayerStats.currentGun.ammoType);
        }
    }

    public void UpdateHealth()
    {
        bar_health.value = m_target.health / m_target.maxHealth;
        // bar_health_fillRect.color = new Color32((byte)(bar_health.value > 0.5f ? (1-bar_health.value)*510 : 255), (byte)(bar_health.value < 0.5f ? (bar_health.value)*510 : 255), 0, 100);
    }

    public void UpdateEnergy()
    {
        bar_energy.value = PlayerStats.energy / PlayerStats.energyMax;
    }

    void Update()
    {
        // update minimap
        {
            Vector2 pos = PlayerStats.rigidbody.position;

            pos /= DynamicLoading.chunkSize;
            pos.x /= DynamicLoading.mapSize.x;
            pos.y /= DynamicLoading.mapSize.y;
            pos.x *= minimap.sizeDelta.x;
            pos.y *= minimap.sizeDelta.y;

            minimapPlayer.localPosition = pos;
        }
    }
}
