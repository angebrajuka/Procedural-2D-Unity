using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    // hierarchy
    public SpriteRenderer m_renderer_body;
    public SpriteRenderer m_renderer_face;
    public SpriteRenderer m_renderer_gun;
    public SpriteRenderer m_renderer_knife;

    // sprites (hierarchy)
    public Sprite[] body;
    public Sprite[] face_happy;
    public Sprite[] face_angry;
    private Sprite[][] faces;

    [HideInInspector] public static PlayerAnimator playerAnimator;


    // inputs
    public enum Mood:byte { HAPPY=0, ANGRY=1}; public static Mood mood;

    void Start() {
        mood = Mood.HAPPY;
        faces = new Sprite[][]{face_happy, face_angry};
        playerAnimator = this;
    }

    void Update() {
        m_renderer_body.sprite = body[PlayerInput.direction8index];
        m_renderer_face.sprite = faces[(byte)mood][PlayerInput.direction8index];
        m_renderer_gun.sprite = PlayerStats.currentGun.sprite;
        m_renderer_gun.flipY = (PlayerInput.angle > 90 && PlayerInput.angle < 270);
        m_renderer_gun.sortingOrder = (PlayerInput.angle < 180 ? 1 : 4);
    }
}
