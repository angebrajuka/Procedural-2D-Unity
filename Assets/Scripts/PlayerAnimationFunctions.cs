using UnityEngine;

public class PlayerAnimationFunctions : MonoBehaviour
{
    public AudioClip punch_miss;
    public AudioClip punch_hit;

    public void OnPunchImpact()
    {
        var direction = ((PlayerAnimator.direction == 0 ? Vector2.right : Vector2.left)+Random.insideUnitCircle.normalized*0.1f).normalized;

        var position = PlayerMovement.rb.position+direction*0.7f;

        RaycastHit2D raycast = Physics2D.Raycast(position, direction, PlayerStats.k_PUNCH_RANGE, Gun.layerMask);

        // Transform trail = MonoBehaviour.Instantiate(Items.guns["pistol"].bulletTrailPrefab, position, Quaternion.identity).transform;
        // trail.gameObject.SetActive(true);
        // trail.GetComponent<BulletTrail>().Init(new Vector3(raycast.collider == null ? PlayerStats.k_PUNCH_RANGE : raycast.distance, 0, 0));

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