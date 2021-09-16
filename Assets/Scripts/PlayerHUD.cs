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
    // public Image bar_health_fillRect;
    public Slider bar_energy;
    public Image minimapImage;
    Sprite mapImageSprite;
    public RectTransform minimap;
    public RectTransform minimapPlayer;

    // components
    Target m_target;

    public void Init()
    {
        m_target = GetComponent<Target>();
        // bar_health_fillRect = bar_health.fillRect.GetComponent<Image>();
        UpdateAmmo();
        UpdateHealth();
    }

    public void SetMapImage(Texture2D texture)
    {
        mapImageSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 122.0f, 1, SpriteMeshType.FullRect, new Vector4(0, 0, texture.width, texture.height), false);
        minimapImage.sprite = mapImageSprite;
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
            ammoImage.sprite = ammoImages[PlayerStats.currentGun.ammoType];
        }

        
        if(PlayerStats.currentItem != null)
        {
            ItemStats item = Items.items[PlayerStats.currentItem];
            currentItem.sprite = item.sprite;
            currentItem.transform.localScale = new Vector3(item.size.x > item.size.y ? 1 : (float)item.size.x/item.size.y, item.size.y > item.size.x ? 1 : (float)item.size.y/item.size.x, 1);
        }
        else
        {
            currentItem.transform.localScale = Vector3.zero;
        }
        currentItem.transform.parent.gameObject.SetActive(PlayerStats.currentItem != null);
    }

    public void UpdateAmmo()
    {
        // smallBulletsTxt.text = PlayerStats.inventory.GetTotalCount(Item.BULLETS_SMALL)+"";
        // bigBulletsTxt.text = PlayerStats.inventory.GetTotalCount(Item.BULLETS_LARGE)+"";
        // shellsTxt.text = PlayerStats.inventory.GetTotalCount(Item.SHELLS)+"";
        // energyTxt.text = PlayerStats.inventory.GetTotalCount(Item.PLASMA)+"";
        if(PlayerStats.currentGun != null)
        {
            currentGunAmmoTxt.text = PlayerStats.GetAmmo() < 10 ? "0" : "";
            currentGunAmmoTxt.text += PlayerStats.GetAmmo()+"";
            // currentReserveTxt.text = "/"+PlayerStats.inventory.GetTotalCount(PlayerStats.currentGun.ammoType);
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

            pos /= ProceduralGeneration.chunkSize;
            pos /= ProceduralGeneration.mapDiameter;
            pos.x *= minimap.sizeDelta.x;
            pos.y *= minimap.sizeDelta.y;

            minimapPlayer.localPosition = pos;
        }
    }
}
