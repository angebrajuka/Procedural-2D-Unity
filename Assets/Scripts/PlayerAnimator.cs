using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public static PlayerAnimator instance;

    // hierarchy
    public SpriteRenderer m_renderer;
    public Animator m_animator;
    public Transform gunSortingGroup;
    public Transform armSortingGroup;
    public SpriteRenderer m_renderer_gun;
    public SpriteRenderer m_renderer_arm;
    public GameObject gun;
    public GameObject arm_right;
    public GameObject arm_left;
    public Transform gunRotatePoint;
    public Transform armRotatePoint;

    // input
    public static int direction=0;

    public void Init()
    {
        instance = this;
    }

    public void BeginMelee()
    {
        m_renderer_gun.enabled = false;
    }

    public void EndMelee()
    {
        m_renderer_gun.enabled = true;
    }

    static readonly int[] convert = {0, 1, 0, 2};
    static readonly Vector3 gunFront = new Vector3(0, -1.001f, 0);
    static readonly Vector3 gunBack  = new Vector3(0, -0.999f, 0);
    static readonly Vector3 armFront  = new Vector3(0, -1.002f, 0);
    static readonly Vector3 armBack  = new Vector3(0, -0.998f, 0);

    void Update()
    {
        m_renderer.flipX = (direction == 1);

        m_animator.SetBool("moving", PlayerMovement.moving);
        m_animator.SetInteger("biome", PlayerMovement.biome);
        m_animator.SetBool("shooting", PlayerState.shooting);
        m_animator.speed = PlayerMovement.moving ? PlayerMovement.rb.velocity.magnitude * 0.07f : 1;

        m_renderer_gun.sprite = PlayerState.currentItem == null || PlayerState.currentItem.gun == null ? null : PlayerState.currentItem.sprite;
        m_renderer_gun.flipY = (PlayerInput.angle > 90 && PlayerInput.angle < 270);
        m_renderer_arm.flipY = m_renderer_gun.flipY;
        gunSortingGroup.localPosition = m_renderer_gun.flipY ? gunBack : gunFront;
        armSortingGroup.localPosition = m_renderer_gun.flipY ? armBack : armFront;
        gunRotatePoint.eulerAngles = new Vector3(0, 0, PlayerInput.angle);
        armRotatePoint.eulerAngles = new Vector3(0, 0, PlayerInput.angle);

        gun.SetActive(PlayerState.shooting);
        arm_right.SetActive(PlayerState.shooting);
        arm_left.SetActive(PlayerState.shooting && direction == 1);
    }
}
