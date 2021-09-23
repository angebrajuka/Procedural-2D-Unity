using UnityEngine;

public class Decoration : MonoBehaviour
{
    public DecorationStats stats;
    public SpriteRenderer sr;
    public Target m_target;
    public float sprite;

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
            m_target = gameObject.AddComponent<Target>();

            var c = gameObject.AddComponent<PolygonCollider2D>();
            c.pathCount = 1;
            c.SetPath(0, stats.collider);
        }
    }

    void Start()
    {
        if(m_target != null)
        {
            m_target.damageable = true;
            m_target.OnDamage = OnDamage;
            m_target.OnKill = OnKill;
            m_target.maxHealth = stats.health*PlayerStats.k_PUNCH_DAMAGE;
            m_target.health = m_target.maxHealth;
        }
    }

    public bool OnDamage(float damage, float angle)
    {
        for(int i=0; i<damage/PlayerStats.k_PUNCH_DAMAGE; i++)
        {
            float val = Random.value;
            for(int j=0; j<stats.itemDrops.Length; j++)
            {
                if(val < stats.itemDrops[j].f)
                {
                    var go = Instantiate(Inventory.instance.itemPickupPrefab, transform.position+new Vector3(stats.size.x/2f, stats.size.y/3f, 0)+Math.Vec3(Random.insideUnitCircle.normalized*0.5f), Quaternion.identity, Entities.t);
                    var pickup = go.GetComponent<ItemPickup>();
                    pickup.Init(stats.itemDrops[j].s, 1);
                    break;
                }
            }

            sprite += stats.sprites.Length / stats.health;
            if(sprite < stats.sprites.Length) sr.sprite = stats.sprites[(int)Mathf.Floor(sprite)];
        }

        return false;
    }

    public bool OnKill(float damage, float angle)
    {
        Destroy(gameObject);

        return false;
    }
}