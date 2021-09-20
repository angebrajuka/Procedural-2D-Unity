using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // hierarchy
    public float debugSpode;

    public static PlayerMovement instance;

    public static Rigidbody2D rb;

    public static bool moving=false;
    public static int biome=0;
    public static float speedMult;

    public void Init()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moving = (PlayerInput.input_move.x != 0 || PlayerInput.input_move.y != 0) && (Mathf.Abs(rb.velocity.x) >= 0.01f || Mathf.Abs(rb.velocity.y) >= 0.01f) && !PauseHandler.paused;
        biome = ProceduralGeneration.MapClamped(ProceduralGeneration.mapTexture_biome, (int)Mathf.Floor(rb.position.x), (int)Mathf.Floor(rb.position.y));
        
        speedMult = 1;
        speedMult *= (ProceduralGeneration.s_shallowWater.Contains(biome) || biome == 0) ? 0.6f : 1;
        speedMult *= PlayerState.sprinting ? PlayerStats.k_SPRINT_MULTIPLIER : (Flashlight.on ? PlayerStats.k_FLASHLIGHT_MULTIPLIER : 1);
        speedMult *= debugSpode;
    }

    void FixedUpdate()
    {
        rb.AddForce(PlayerInput.input_move * speedMult * PlayerStats.k_RUN_ACCELL);
    }
}