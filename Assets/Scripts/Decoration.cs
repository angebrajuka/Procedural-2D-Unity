using UnityEngine;

public class Decoration : MonoBehaviour
{
    public DecorationStats stats;
    public SpriteRenderer sr;
    public Target m_target;
    public Vector2Int[] availableMasks;
    public int size;

    public void Init(DecorationStats stats)
    {
        this.stats = stats;

        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = stats.sortingLayer;
        sr.sprite = stats.sprite;

        if(stats.collider != null)
        {
            m_target = gameObject.AddComponent<Target>();

            var c = gameObject.AddComponent<PolygonCollider2D>();
            c.pathCount = 1;
            c.SetPath(0, stats.collider);
        }

        size = stats.renderSize.x*stats.renderSize.y;
        availableMasks = new Vector2Int[size];
        int i=0;
        for(int x=0; x<stats.renderSize.x; x++) for(int y=0; y<stats.renderSize.y; y++)
        {
            availableMasks[i] = new Vector2Int(x, y);
            i++;
        }
    }

    void Start()
    {
        if(m_target != null)
        {
            m_target.damageable = stats.health > 0;
            m_target.OnDamage = OnDamage;
            m_target.OnKill = OnKill;
            m_target.maxHealth = stats.health*PlayerStats.k_PUNCH_DAMAGE;
            m_target.health = m_target.maxHealth;
        }
    }

    void Drop()
    {
        float val = Random.value;
        for(int j=0; j<stats.itemDrops.Length; j++)
        {
            if(val < stats.itemDrops[j].f)
            {
                var go = Instantiate(Inventory.instance.itemPickupPrefab, transform.position+new Vector3(stats.size.x/2f, stats.size.y/3f, 0)+Math.Vec3(Random.insideUnitCircle.normalized*0.5f), Quaternion.identity, Entities.t);
                var pickup = go.GetComponent<ItemPickup>();
                pickup.Init(stats.itemDrops[j].s, 1, 0, true);
                break;
            }
        }
    }

    public bool OnDamage(float damage, float angle)
    {
        for(int i=0; i<damage/PlayerStats.k_PUNCH_DAMAGE; i++)
        {
            int vLength = availableMasks.Length;
            for(int j=0, k=0; j<size*1.5f/stats.health; j++)
            {
                if(vLength == 0) break;
                k = Random.Range(0, vLength); // min inclusive, max exclusive
                var mask = Instantiate(ProceduralGeneration.instance.prefab_mask, transform.position+Math.Vec3(availableMasks[k]), Quaternion.identity, transform);
                mask.GetComponent<SpriteMask>().sprite = ProceduralGeneration.instance.sprite_masks[Random.Range(0, ProceduralGeneration.instance.sprite_masks.Length)];
                availableMasks[k] = availableMasks[--vLength];
            }
            System.Array.Resize(ref availableMasks, vLength);

            Drop();
        }

        return false;
    }

    public bool OnKill(float damage, float angle)
    {
        for(int i=0; i<5; i++)
        {
            Drop();
        }

        Destroy(gameObject);

        return false;
    }
}