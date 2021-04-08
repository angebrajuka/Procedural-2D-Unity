using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Target : MonoBehaviour
{
    public float maxHealth = 100;
    public float health = 100;
    public bool damageable = false;
    public bool dead = false;

    public abstract void OnDamage(float damage);
    public abstract void OnKill(float damage);
    public abstract void OnHeal(float heal);
    
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
