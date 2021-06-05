using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Bomb : MonoBehaviour {

    void Start() {
        Destroy(gameObject, 2);
    }

    void OnDestroy() {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.DetachChildren();
    }
}
