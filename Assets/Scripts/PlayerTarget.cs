using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    public static Target target;
    [HideInInspector] public PlayerHUD m_playerHUD;

    public void Init()
    {
        target = GetComponent<Target>();
        m_playerHUD = GetComponent<PlayerHUD>();
        target.OnDamage = OnDamage;
        target.OnKill = OnKill;
        target.OnHeal = OnHeal;
    }

    public bool OnDamage(float damage, float angle=0)
    {
        m_playerHUD.UpdateHealth();
        return true;
    }

    public bool OnKill(float damage, float angle=0)
    {
        m_playerHUD.UpdateHealth();
        return true;
    }

    public bool OnHeal(float heal)
    {
        m_playerHUD.UpdateHealth();
        return true;
    }
}
