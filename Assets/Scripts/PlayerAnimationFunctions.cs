using UnityEngine;

public class PlayerAnimationFunctions : MonoBehaviour
{
    public AudioClip punch_miss;
    public AudioClip punch_hit;

    public void OnPunchImpact()
    {
        var direction = ((PlayerAnimator.direction == 0 ? Vector2.right : Vector2.left)+Random.insideUnitCircle.normalized/3).normalized;

        RaycastHit2D raycast = Physics2D.Raycast(PlayerMovement.rb.position+direction, direction, PlayerStats.k_PUNCH_RANGE, Gun.layerMask);

        if(raycast.collider == null)
        {
            AudioManager.PlayClip(punch_miss);
        }
        else
        {
            var target = raycast.transform.GetComponent<Target>();
            if(target != null)
            {
                target.Damage(PlayerStats.k_PUNCH_DAMAGE, Math.NormalizedVecToAngle(direction));
                AudioManager.PlayClip(punch_hit);
            }
        }
    }

    public void OnPunchEnd()
    {
        PlayerState.punching = false;
    }
}