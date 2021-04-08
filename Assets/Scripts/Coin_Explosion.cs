using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Explosion : MonoBehaviour {
    
    // hierarchy
    public Transform prefab_coin;
    public int count;
    public int vel;

    void Start() {
        
        for(int i=0; i<count; i++) {
            Transform coin = Instantiate(prefab_coin, transform.position, Quaternion.identity);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2((Random.value-0.5f)*vel, (Random.value-0.5f)*vel));
        }

        Destroy(this.gameObject);
    }
}
