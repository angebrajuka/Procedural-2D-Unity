using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{    
    static readonly Vector2 vecLength2 = new Vector2(2, 2);

    // hierarchy
    public string item;

    // components
    [HideInInspector] public Rigidbody2D m_rigidbody;

    public int count;
    public int ammo=0;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
        {
            PlayerHUD.instance.interact.text = "press   <F>   to   loot";// + Items.names[(int)item];
            PlayerState.interactItem = item;
            PlayerState.interactPickup = this;
        }
        else if(other.gameObject.layer == gameObject.layer)
        {
            Rigidbody2D o_rigidbody = other.gameObject.GetComponent<Rigidbody2D>();

            Vector3 delta = (vecLength2-(o_rigidbody.position - m_rigidbody.position))*1.3f;
            o_rigidbody.AddForce(-delta);
            m_rigidbody.AddForce(delta);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer != 8) return;
        PlayerHUD.instance.interact.text = "";
        PlayerState.interactItem = null;
        PlayerState.interactPickup = null;
    }
}
