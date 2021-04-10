﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {
    
    // hierarchy
    public Item item;

    // components
    Rigidbody2D m_rigidbody;

    static readonly string[] strings = {    "NONE",
                                            "BLADE",
                                            "BOMB",
                                            "MEDKIT",
                                            "STIMPACK",
                                            "COMPASS",
                                            "POTION" };

    void Start() {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    public bool Condition() {
        switch(item) {
            case Item.BLADE:
                return m_rigidbody.velocity.magnitude < 3;
            default:
                return true;
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if(!Condition()) return;
        if(other.gameObject.layer == 8) {
            PlayerStats.playerStats.playerHUD.interact.text = "press <E> for " + strings[(int)item];
            PlayerStats.interactItem = item;
            PlayerStats.interactPickup = this;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.layer != 8) return;
        PlayerStats.playerStats.playerHUD.interact.text = "";
        PlayerStats.interactItem = Item.NONE;
        PlayerStats.interactPickup = null;
    }
}
