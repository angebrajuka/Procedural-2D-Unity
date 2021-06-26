using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    [HideInInspector] public Target m_target;
    [HideInInspector] public PlayerHUD m_playerHUD;

    void Start()
    {
        m_target = GetComponent<Target>();
        m_playerHUD = GetComponent<PlayerHUD>();
        m_target.OnDamage = OnDamage;
        m_target.OnKill = OnKill;
        m_target.OnHeal = OnHeal;
    }

    public bool OnDamage(float damage)
    {
        m_playerHUD.UpdateHealth();
        return true;
    }

    public bool OnKill(float damage)
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
