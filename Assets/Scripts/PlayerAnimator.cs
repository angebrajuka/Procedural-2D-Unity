﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    [SerializeField] private float speed_runAnimation;
    [SerializeField] private SpriteRenderer m_renderer;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform gunSortingGroup, armSortingGroup;
    [SerializeField] private SpriteRenderer m_renderer_gun, m_renderer_arm, m_renderer_sword;
    [SerializeField] private GameObject gun, arm_right, arm_left;
    [SerializeField] private Transform gunRotatePoint, armRotatePoint;

    public void BeginMelee() {
        m_renderer_gun.enabled = false;
    }

    public void EndMelee() {
        m_renderer_gun.enabled = true;
    }

    static readonly int[] convert = {0, 1, 0, 2};
    static readonly Vector3 gunFront = new Vector3(0, -1.001f, 0);
    static readonly Vector3 gunBack  = new Vector3(0, -0.999f, 0);
    static readonly Vector3 armFront  = new Vector3(0, -1.002f, 0);
    static readonly Vector3 armBack  = new Vector3(0, -0.998f, 0);

    static Vector3 playerAngle = new Vector3(0, 0, 0);

    public void UpdateMovement(bool flipped, bool moving, float speed) {
        m_renderer.flipX = flipped;
        m_animator.SetBool("moving", moving);
        m_animator.speed = moving ? speed * speed_runAnimation : 1;
    }

    void Update() {

        m_animator.SetInteger("biome", 2);
        // m_animator.SetBool("shooting", PlayerState.shooting || PlayerState.melee);
        // m_animator.SetBool("punching", PlayerState.punching);

        // m_renderer_gun.sprite = PlayerState.currentGun == null ? null : PlayerState.currentItem.sprite;
        // m_renderer_gun.flipY = (PlayerInput.angle > 90 && PlayerInput.angle < 270);
        // m_renderer_arm.flipY = m_renderer_gun.flipY;
        // m_renderer_sword.gameObject.SetActive(PlayerState.melee);
        // m_renderer_sword.flipX = m_renderer_gun.flipY;
        // var swordPos = m_renderer_sword.transform.localPosition;
        // swordPos.y = Mathf.Abs(swordPos.y)*(m_renderer_sword.flipX ? -1 : 1);
        // m_renderer_sword.transform.localPosition = swordPos;
        // var swordAngle = m_renderer_sword.transform.localEulerAngles;
        // swordAngle.z = m_renderer_sword.flipX ? -100 : -80;
        // m_renderer_sword.transform.localEulerAngles = swordAngle;
        // gunSortingGroup.localPosition = m_renderer_gun.flipY ? gunBack : gunFront;
        // armSortingGroup.localPosition = m_renderer_gun.flipY ? armBack : armFront;
        // playerAngle.z = PlayerInput.angle;
        // gunRotatePoint.eulerAngles = playerAngle;
        // armRotatePoint.eulerAngles = playerAngle;

        // gun.SetActive(PlayerState.shooting);
        // arm_right.SetActive(PlayerState.shooting || PlayerState.melee);
        // arm_left.SetActive((PlayerState.shooting || PlayerState.melee) && direction == 1);
    }
}
