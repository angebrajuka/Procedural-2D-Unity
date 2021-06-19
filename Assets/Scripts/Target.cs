using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float maxHealth = 100;
    public float health = 100;
    public bool damageable = false;
    public bool dead = false;

    public static bool Default(float f) { return false; }

    public Func<float, bool> OnDamage=Default;
    public Func<float, bool> OnKill=Default;
    public Func<float, bool> OnHeal=Default;
    
    public bool Damage(float damage) {
        if(damageable && !dead) {
            health -= damage;
            OnDamage(damage);
            if(health <= 0) {
                dead = true;
                OnKill(damage);
            }
            return true;
        }
        OnDamage(0);
        return false;
    }

    public bool Heal(float heal) {
        if(damageable) {
            health += heal;
            OnHeal(heal);
            if(health > maxHealth) {
                health = maxHealth;
                return true;
            }
        }
        return false;
    }
}
