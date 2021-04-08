using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Target {
    public override void OnDamage(float damage) {}
    public override void OnKill(float damage) { Destroy(this.gameObject); }
    public override void OnHeal(float heal) {}
}
