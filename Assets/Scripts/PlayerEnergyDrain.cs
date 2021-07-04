using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergyDrain : MonoBehaviour
{
    void Update()
    {
        if(DaylightCycle.time > DaylightCycle.k_DAY && DaylightCycle.time < DaylightCycle.k_EVENING && PlayerStats.energy < PlayerStats.energyMax)
        {
            PlayerStats.energy += Time.deltaTime;
            PlayerStats.energy = Mathf.Min(PlayerStats.energy, PlayerStats.energyMax);
            PlayerStats.hud.UpdateEnergy();
        }
        else if(PlayerStats.energy > 0)
        {
            PlayerStats.energy -= Time.deltaTime * ((PlayerStats.currentItem == Item.FLASHLIGHT ? 1 : 0) + (PlayerStats.sprinting ? 1 : 0));
            PlayerStats.energy = Mathf.Max(PlayerStats.energy, 0);
            PlayerStats.hud.UpdateEnergy();
        }
    }
}
