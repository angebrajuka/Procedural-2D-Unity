using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // hierarchy
    public EightDirectionAnimator m_animator_body;
    public SpriteRenderer m_renderer_gun;

    // static reference
    [HideInInspector] public static PlayerAnimator playerAnimator;

    public void BeginMelee()
    {
        m_renderer_gun.enabled = false;
    }

    public void EndMelee()
    {
        m_renderer_gun.enabled = true;
    }

    void Start()
    {
        playerAnimator = this;
    }

    public void UpdateGunImage()
    {
        m_renderer_gun.sprite = PlayerStats.currentGun == null ? null : PlayerStats.currentGun.sprite;
    }

    void Update()
    {
        m_animator_body.direction = PlayerInput.direction8index;
        m_renderer_gun.flipY = (PlayerInput.angle > 90 && PlayerInput.angle < 270);
    }
}
