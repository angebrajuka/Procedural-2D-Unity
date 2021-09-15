using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // hierarchy
    public SpriteRenderer m_renderer;
    public Animator m_animator;
    public SpriteRenderer m_renderer_gun;
    public SpriteRenderer[] m_renderers_guns;

    // input
    public static int direction=0;

    public void BeginMelee()
    {
        if(m_renderer_gun != null) m_renderer_gun.enabled = false;
    }

    public void EndMelee()
    {
        if(m_renderer_gun != null) m_renderer_gun.enabled = true;
    }

    public void UpdateGunImage()
    {
        if(m_renderer_gun != null) m_renderer_gun.enabled = false;
        m_renderer_gun = PlayerStats._currentGun == -1 ? null : m_renderers_guns[PlayerStats._currentGun];
        if(m_renderer_gun != null) m_renderer_gun.enabled = true;
    }

    static readonly int[] convert = {0, 1, 0, 2};

    void Update()
    {
        if(m_renderer_gun != null)
        {
            m_renderer_gun.flipY = (PlayerInput.angle > 90 && PlayerInput.angle < 270);
            Vector3 position = PlayerStats.currentGun.barrelTip.localPosition;
            position.y = Mathf.Abs(position.y) * (m_renderer_gun.flipY ? -1 : 1);
            PlayerStats.currentGun.barrelTip.localPosition = position;
        }

        bool moving = (PlayerInput.input_move.x != 0 || PlayerInput.input_move.y != 0) && (Mathf.Abs(PlayerStats.rigidbody.velocity.x) >= 0.01f || Mathf.Abs(PlayerStats.rigidbody.velocity.y) >= 0.01f) && !PauseHandler.paused;
        int biome = ProceduralGeneration.MapClamped(ProceduralGeneration.mapTexture_biome, (int)Mathf.Floor(PlayerStats.rigidbody.position.x), (int)Mathf.Floor(PlayerStats.rigidbody.position.y));
        PlayerStats.speedMult = (ProceduralGeneration.s_shallowWater.Contains(biome) || biome == 0) ? 0.6f : 1;
        m_animator.SetBool("moving", moving);
        m_animator.SetInteger("biome", biome);
        m_animator.speed = moving ? PlayerStats.rigidbody.velocity.magnitude * 0.07f : 1;
        

        m_renderer.flipX = (direction == 1);
    }
}
