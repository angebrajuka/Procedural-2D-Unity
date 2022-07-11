using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Singles;

public class PlayerMovement : MonoBehaviour {
    // hierarchy
    public float debugSpode;
    public WorldGen worldGen;
    public Follow cameraFollow;

    public static Rigidbody2D rb;
    Collider2D c2d;
    float halfColliderHeight;

    public static bool moving=false;
    private bool inWater;
    public bool InWater { get { return inWater; } }
    private bool inOcean;
    public bool InOcean { get { return inOcean; } }
    public static float speedMult;
    static Vector2 input_move;

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        c2d = GetComponent<Collider2D>();
        halfColliderHeight = (c2d.bounds.max.y - c2d.bounds.min.y) / 2f;
    }

    public void StartGame() {
        transform.position = worldGen.playerSpawnPoint; // transform.position is updated instantly, rb.position wont update till next frame, caused loading bug
        cameraFollow.toFollow = transform;
        cameraFollow.offset = Vector3.zero;
        cameraFollow.Snap();
    }

    public void tp(float x, float y) {
        var pos = transform.position;
        pos.x = x;
        pos.y = y;
        transform.position = pos;
    }

    void Update() {
        input_move.x = 0;
        input_move.y = 0;
        if(PlayerInput.GetKey("move east" )) input_move.x ++;
        if(PlayerInput.GetKey("move north")) input_move.y ++;
        if(PlayerInput.GetKey("move west" )) input_move.x --;
        if(PlayerInput.GetKey("move south")) input_move.y --;
        input_move.Normalize();

        PlayerAnimator.direction = (input_move.x == 0) ? (Input.mousePosition.x > (Screen.width / 2) ? 0 : 1) : (input_move.x > 0 ? 0 : 1);

        moving = (input_move.x != 0 || input_move.y != 0) && (Mathf.Abs(rb.velocity.x) >= 0.01f || Mathf.Abs(rb.velocity.y) >= 0.01f) && !PauseHandler.paused;

        int x = (int)Mathf.Floor(rb.position.x+c2d.offset.x),
            y = (int)Mathf.Floor(rb.position.y+c2d.offset.y-halfColliderHeight);
        inWater = worldGen.IsWater(x,y);
        inOcean = worldGen.IsOcean(x,y);

        speedMult = 1;
        speedMult *= InWater ? 0.6f : 1;
        speedMult *= PlayerState.sprinting ? PlayerStats.k_SPRINT_MULTIPLIER : (Flashlight.on ? PlayerStats.k_FLASHLIGHT_MULTIPLIER : 1);
        speedMult *= debugSpode;
    }

    void FixedUpdate() {
        rb.AddForce(input_move * speedMult * PlayerStats.k_RUN_ACCELL);
    }
}