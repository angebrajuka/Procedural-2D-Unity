using UnityEngine;

public class Decoration : MonoBehaviour
{
    public DecorationStats stats;
    public SpriteRenderer sr;
    public int sprite;

    public void Init(DecorationStats stats)
    {
        this.stats = stats;

        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = stats.sortingLayer;
        sr.spriteSortPoint = SpriteSortPoint.Pivot;
        sr.material = ProceduralGeneration.instance.material;
        sprite = 0;
        sr.sprite = stats.sprites[0];

        if(stats.collider != null)
        {
            var target = gameObject.AddComponent<Target>();
            target.damageable = true;
            target.OnDamage = OnDamage;
            target.OnKill = OnKill;
            target.maxHealth = stats.sprites.Length*PlayerStats.k_PUNCH_DAMAGE;
            target.health = target.maxHealth;

            var c = gameObject.AddComponent<PolygonCollider2D>();
            c.pathCount = 1;
            c.SetPath(0, stats.collider);
        }
    }

    public bool OnDamage(float damage, float angle)
    {
        sprite += (int)Mathf.Round(damage/PlayerStats.k_PUNCH_DAMAGE);
        if(sprite < stats.sprites.Length) sr.sprite = stats.sprites[sprite];

        return false;
    }

    public bool OnKill(float damage, float angle)
    {
        Destroy(gameObject);

        return false;
    }
}