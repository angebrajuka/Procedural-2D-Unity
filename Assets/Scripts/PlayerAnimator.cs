using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    // hierarchy
    public EightDirectionAnimator m_animator_body;
    public EightDirectionAnimator m_animator_face;
    public SpriteRenderer m_renderer_gun;


    // static reference
    [HideInInspector] public static PlayerAnimator playerAnimator;


    // inputs
    public enum Mood:byte { HAPPY=0, ANGRY=1}; public static Mood mood;

    void Start() {
        mood = Mood.HAPPY;
        // faces = new Sprite[][]{face_happy, face_angry};
        playerAnimator = this;
    }

    void Update() {
        // m_renderer_body.sprite = body[PlayerInput.direction8index];
        // m_renderer_face.sprite = faces[(byte)mood][PlayerInput.direction8index];
        m_animator_body.direction = PlayerInput.direction8index;
        m_animator_face.direction = PlayerInput.direction8index;
        m_animator_face.state = (int)mood;
        m_renderer_gun.sprite = PlayerStats.currentGun.sprite;
        m_renderer_gun.flipY = (PlayerInput.angle > 90 && PlayerInput.angle < 270);
        m_renderer_gun.sortingOrder = (PlayerInput.angle < 180 ? 3 : 6);
    }
}
