using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        Target target = collider.transform.GetComponent<Target>();
        if(target != null)
        {
            target.Damage(PlayerStats.g_KNIFE_DAMAGE, PlayerInput.angle);
        }
    }
}
