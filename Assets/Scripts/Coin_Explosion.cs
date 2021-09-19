using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Explosion : MonoBehaviour
{    
    // hierarchy
    public Transform prefab_coin;
    public int count;
    public int variation;
    public int vel;

    void Start()
    {
        count += (int)((Random.value-0.5)*variation);

        for(int i=0; i<count; i++)
        {
            Transform coin = Instantiate(prefab_coin, transform.position, Quaternion.identity, Entities.t);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2((Random.value-0.5f)*vel, (Random.value-0.5f)*vel));
        }

        Destroy(this.gameObject);
    }
}
