using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{    
    static readonly Vector2 vecLength2 = new Vector2(2, 2);

    // hierarchy
    public string item;

    // components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;

    public int count;
    public int ammo=0;

    public void Init(string item, int count)
    {
        this.item = item;
        this.count = count;
        GetComponent<Rigidbody2D>().AddForce(new Vector2((Random.value-0.5f)*100, (Random.value-0.5f)*100));
    }

    void Start()
    {
        if(!Items.items.ContainsKey(item))
        {
            Debug.Log("item no exist");
            Destroy(gameObject);
            return;
        }
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = Items.items[item].sprite;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
        {
            PlayerHUD.instance.interact.text = "press <F> to loot";// + Items.names[(int)item];
            PlayerState.interactItem = item;
            PlayerState.interactPickup = this;
        }
        // else if(other.gameObject.layer == gameObject.layer)
        // {
        //     Rigidbody2D o_rigidbody = other.gameObject.GetComponent<Rigidbody2D>();

        //     var delta = (vecLength2-(o_rigidbody.position - rb.position))*1.3f;
        //     o_rigidbody.AddForce(-delta+Random.insideUnitCircle);
        //     rb.AddForce(delta);
        // }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer != 8) return;
        PlayerHUD.instance.interact.text = "";
        PlayerState.interactItem = null;
        PlayerState.interactPickup = null;
    }
}
