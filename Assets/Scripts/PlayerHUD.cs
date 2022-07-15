using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;

    // inspector
new public Transform transform;
    public Text currentGunAmmoTxt;
    public Text currentReserveTxt;
    public Text interact;
    public Image ammoImage;
    public Image currentItem;
    public Text currentItemCount;
    public RectTransform ammoTxt;
    public Slider bar_health;
    // public Image bar_health_fillRect;
    public Slider bar_energy;
    public Image minimapImage;
    Sprite mapImageSprite;
    public RectTransform minimap;
    public RectTransform minimapPlayer;

    // components
    Target m_target;
    Dictionary<string, Sprite> ammoImages;

    public void Init()
    {
        instance = this;

        m_target = GetComponent<Target>();
        ammoImages = new Dictionary<string, Sprite>();
        foreach(var pair in Items.guns)
        {
            if(!ammoImages.ContainsKey(pair.Value.ammoType))
            {
                ammoImages.Add(pair.Value.ammoType, Items.items[pair.Value.ammoType].sprite);
            }
        }
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
        ammoTxt.gameObject.SetActive(false);
        currentItem.transform.localScale = Vector3.zero;

        if(PlayerState.currentGun != null)
        {
            ammoTxt.gameObject.SetActive(true);
            ammoImage.sprite = ammoImages[PlayerState.currentGun.ammoType];
        }
        else if(PlayerState.currentItem != null)
        {
            currentItem.sprite = PlayerState.currentItem.sprite;
            currentItem.transform.localScale = new Vector3(PlayerState.currentItem.size.x > PlayerState.currentItem.size.y ? 1 : (float)PlayerState.currentItem.size.x/PlayerState.currentItem.size.y, PlayerState.currentItem.size.y > PlayerState.currentItem.size.x ? 1 : (float)PlayerState.currentItem.size.y/PlayerState.currentItem.size.x, 1);
            
            int count = Inventory.instance.GetTotalCount(PlayerState.currentItem.name);
            currentItemCount.text = count == 1 ? "" : count+"";
        }

        currentItem.transform.parent.gameObject.SetActive(PlayerState.currentItem != null && PlayerState.currentGun == null);
    }

    public void UpdateAmmo()
    {
        if(PlayerState.currentGun != null)
        {
            currentGunAmmoTxt.text = PlayerStats.GetAmmo() < 10 ? "0" : "";
            currentGunAmmoTxt.text += PlayerStats.GetAmmo()+"";
            currentReserveTxt.text = "/"+Inventory.instance.GetTotalCount(PlayerState.currentGun.ammoType);
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
            Vector2 pos = PlayerMovement.rb.position;

            pos /= World.diameter;
            pos.x *= minimap.sizeDelta.x;
            pos.y *= minimap.sizeDelta.y;

            minimapPlayer.localPosition = pos;
        }
    }
}
