using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // hierarchy
    public EightDirectionAnimator m_animator_body;
    public SpriteRenderer m_renderer_gun;
    public SpriteRenderer[] m_renderers_guns;

    // static reference
    [HideInInspector] public static PlayerAnimator playerAnimator;

    public void BeginMelee()
    {
        if(m_renderer_gun != null) m_renderer_gun.enabled = false;
    }

    public void EndMelee()
    {
        if(m_renderer_gun != null) m_renderer_gun.enabled = true;
    }

    void Start()
    {
        playerAnimator = this;
    }

    public void UpdateGunImage()
    {
        if(m_renderer_gun != null) m_renderer_gun.enabled = false;
        m_renderer_gun = PlayerStats._currentGun == -1 ? null : m_renderers_guns[PlayerStats._currentGun];
        if(m_renderer_gun != null) m_renderer_gun.enabled = true;
    }

    void Update()
    {
        m_animator_body.direction = PlayerInput.direction8index;
        if(m_renderer_gun != null)
        {
            m_renderer_gun.flipY = (PlayerInput.angle > 90 && PlayerInput.angle < 270);
            Vector3 position = PlayerStats.currentGun.barrelTip.localPosition;
            position.y = Mathf.Abs(position.y) * (m_renderer_gun.flipY ? -1 : 1);
            PlayerStats.currentGun.barrelTip.localPosition = position;
        }
    }
}
