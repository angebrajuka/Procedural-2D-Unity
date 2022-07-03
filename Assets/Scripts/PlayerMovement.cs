using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Singles;

public class PlayerMovement : MonoBehaviour {
    // hierarchy
    public float debugSpode;

    public static Rigidbody2D rb;

    public static bool moving=false;
    public static int biome=0;
    public static float speedMult;
    static Vector2 input_move;

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartGame() {
        transform.position = WorldGen.playerSpawnPoint; // transform.position is updated instantly, rb.position wont update till next frame, caused loading bug
        singles.cameraFollow.toFollow = transform;
        singles.cameraFollow.offset = Vector3.zero;
        singles.cameraFollow.Snap();
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
        biome = WorldGen.GetTile((int)Mathf.Floor(rb.position.x), (int)Mathf.Floor(rb.position.y));

        speedMult = 1;
        speedMult *= (WorldGen.s_shallowWater.Contains(biome) || biome == 0) ? 0.6f : 1;
        speedMult *= PlayerState.sprinting ? PlayerStats.k_SPRINT_MULTIPLIER : (Flashlight.on ? PlayerStats.k_FLASHLIGHT_MULTIPLIER : 1);
        speedMult *= debugSpode;
    }

    void FixedUpdate() {
        rb.AddForce(input_move * speedMult * PlayerStats.k_RUN_ACCELL);
    }
}