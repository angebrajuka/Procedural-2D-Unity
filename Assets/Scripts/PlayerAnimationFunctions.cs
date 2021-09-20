using UnityEngine;

public class PlayerAnimationFunctions : MonoBehaviour
{
    public void OnPunchImpact()
    {
        var direction = Math.AngleToVector2(PlayerInput.angle);

        const int layerMask = ~(1<<8 | 1<<2 | 1<<10 | 1<<12); // 8 to ignore player, 2 to ignore ignore raycast, 10 to ignore ground, 12 to ignore knife
        RaycastHit2D raycast = Physics2D.Raycast(PlayerMovement.rb.position+direction, direction, PlayerStats.k_PUNCH_RANGE, layerMask);

        if(raycast.collider != null)
        {    
            var target = raycast.transform.GetComponent<Target>();

            if(target != null)
            {
                target.Damage(PlayerStats.k_PUNCH_DAMAGE, PlayerInput.angle);
            }
        }
    }

    public void OnPunchEnd()
    {
        PlayerState.punching = false;
    }
}