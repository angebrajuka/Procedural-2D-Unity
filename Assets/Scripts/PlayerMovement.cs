using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static bool moving=false;
    public static int biome=0;
    public static float speedMult;

    void Update()
    {
        moving = (PlayerInput.input_move.x != 0 || PlayerInput.input_move.y != 0) && (Mathf.Abs(PlayerStats.rigidbody.velocity.x) >= 0.01f || Mathf.Abs(PlayerStats.rigidbody.velocity.y) >= 0.01f) && !PlayerState.shooting && !PauseHandler.paused;
        biome = ProceduralGeneration.MapClamped(ProceduralGeneration.mapTexture_biome, (int)Mathf.Floor(PlayerStats.rigidbody.position.x), (int)Mathf.Floor(PlayerStats.rigidbody.position.y));
        
        speedMult = 1;
        speedMult *= (ProceduralGeneration.s_shallowWater.Contains(biome) || biome == 0) ? 0.6f : 1;
        speedMult *= PlayerState.sprinting ? PlayerStats.k_SPRINT_MULTIPLIER : (Flashlight.on ? PlayerStats.k_FLASHLIGHT_MULTIPLIER : 1);
        speedMult *= PlayerStats.instance.debugSpode;
        // speedMult *= PlayerInput.shooting ? 0.4f : 1;
    }

    void FixedUpdate()
    {
        PlayerStats.rigidbody.AddForce(PlayerInput.input_move * speedMult * PlayerStats.k_RUN_ACCELL);
    }
}