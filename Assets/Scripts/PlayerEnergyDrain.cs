using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyDrain : MonoBehaviour
{
    void Update()
    {
        PlayerStats.energy += ((DaylightCycle.time > DaylightCycle.k_DAY && DaylightCycle.time < DaylightCycle.k_EVENING && PlayerStats.energy < PlayerStats.energyMax) ? Time.deltaTime : 0)
                            - ((PlayerStats.energy > 0) ? (Time.deltaTime * ((Flashlight.on ? 2 : 0) + (PlayerState.sprinting ? 5 : 0))) : 0);

        PlayerStats.energy = Mathf.Clamp(PlayerStats.energy, 0, PlayerStats.energyMax);
        PlayerHUD.instance.UpdateEnergy();
    }
}
