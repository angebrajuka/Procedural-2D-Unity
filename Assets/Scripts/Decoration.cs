using UnityEngine;

public class Decoration : MonoBehaviour
{
    public Sprite[] sprites;
    public Target m_target;
    public SpriteRenderer sr;
    public int sprite;

    public void Init(string name)
    {
        sprites = Resources.LoadAll<Sprite>("sprites/decorations/"+name);
        m_target = GetComponent<Target>();

        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = Biome.s_altSortingOrders.ContainsKey(name) ? Biome.s_altSortingOrders[name] : 1;
        sr.spriteSortPoint = SpriteSortPoint.Pivot;
        sr.material = ProceduralGeneration.instance.material;
        
        if(Biome.s_colliders.ContainsKey(name))
        {
            var c = gameObject.AddComponent<PolygonCollider2D>();
            c.pathCount = 1;
            c.SetPath(0, Biome.s_colliders[name]);
        }

        Biome.s_decorations.Add(name, gameObject);
        Biome.s_decorationSizes.Add(name, new Vector2Int((int)sprites[0].rect.width/(int)sprites[0].pixelsPerUnit, (int)Mathf.Ceil(sprites[0].rect.height/2f/sprites[0].pixelsPerUnit)));
    }

    void Start()
    {
        m_target.damageable = true;
        m_target.OnDamage = OnDamage;
        m_target.OnKill = OnKill;
        m_target.maxHealth = sprites.Length*PlayerStats.k_PUNCH_DAMAGE;
        m_target.health = m_target.maxHealth;
        sprite = 0;
        sr.sprite = sprites[0];
    }

    public bool OnDamage(float damage, float angle)
    {
        sprite += (int)Mathf.Round(damage/PlayerStats.k_PUNCH_DAMAGE);
        if(sprite < sprites.Length) sr.sprite = sprites[sprite];

        return false;
    }

    public bool OnKill(float damage, float angle)
    {
        Destroy(gameObject);

        return false;
    }
}