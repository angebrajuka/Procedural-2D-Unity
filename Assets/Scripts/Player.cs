﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Target {

    public PlayerHUD playerHUD;

    void Start() {
        PlayerStats.player = this;
    }

    public override void OnDamage(float damage) {
        playerHUD.UpdateHealth();
    }

    public override void OnKill(float damage) {
        playerHUD.UpdateHealth();
    }

    public override void OnHeal(float heal) {
        playerHUD.UpdateHealth();
    }
}
