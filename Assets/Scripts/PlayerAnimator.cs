using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // hierarchy
    public SpriteRenderer m_renderer;
    public Animator m_animator;
    public Transform weaponSortingGroup;
    public SpriteRenderer m_renderer_gun;

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
        // m_renderer_gun.sprite = PlayerStats._currentGun == -1 ? null : PlayerStats.currentGun.sprite;
    }

    static readonly int[] convert = {0, 1, 0, 2};
    static readonly Vector3 weaponFront = new Vector3(0, -1.01f, 0);
    static readonly Vector3 weaponBack  = new Vector3(0, -0.99f, 0);

    void Update()
    {
        m_renderer.flipX = (direction == 1);
        
        // if(PlayerStats.currentGun != null)
        // {
            weaponSortingGroup.localPosition = m_renderer.flipX ? weaponBack : weaponFront;
            m_renderer_gun.flipY = (PlayerInput.angle > 90 && PlayerInput.angle < 270);
            // Vector3 position = PlayerStats.currentGun.barrelTip;
            // position.y = Mathf.Abs(position.y) * (m_renderer_gun.flipY ? -1 : 1);
            // PlayerStats.currentGun.barrelTip = position;
        // }

        bool moving = (PlayerInput.input_move.x != 0 || PlayerInput.input_move.y != 0) && (Mathf.Abs(PlayerStats.rigidbody.velocity.x) >= 0.01f || Mathf.Abs(PlayerStats.rigidbody.velocity.y) >= 0.01f) && !PauseHandler.paused;
        int biome = ProceduralGeneration.MapClamped(ProceduralGeneration.mapTexture_biome, (int)Mathf.Floor(PlayerStats.rigidbody.position.x), (int)Mathf.Floor(PlayerStats.rigidbody.position.y));
        PlayerStats.speedMult = (ProceduralGeneration.s_shallowWater.Contains(biome) || biome == 0) ? 0.6f : 1;
        m_animator.SetBool("moving", moving);
        m_animator.SetInteger("biome", biome);
        m_animator.speed = moving ? PlayerStats.rigidbody.velocity.magnitude * 0.07f : 1;
    }
}
