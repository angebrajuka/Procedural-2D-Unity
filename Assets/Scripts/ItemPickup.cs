using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{    
    // hierarchy
    public Item item;

    // components
    Rigidbody2D m_rigidbody;

    public int count;
    public int ammo=0;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
        {
            PlayerStats.hud.interact.text = "press   <F>   to   loot";// + Items.names[(int)item];
            PlayerStats.interactItem = item;
            PlayerStats.interactPickup = this;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer != 8) return;
        PlayerStats.hud.interact.text = "";
        PlayerStats.interactItem = Item.NONE;
        PlayerStats.interactPickup = null;
    }
}
