using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    // hierarchy
    public EnemyObject enemyObject;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != 8 && other.gameObject.layer != 11 && !enemyObject.nearbyColliders.ContainsKey(other.transform))
        {
            enemyObject.nearbyColliders.Add(other.transform, Math.Vec2(other.bounds.min+other.bounds.max)/2);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(enemyObject.nearbyColliders.ContainsKey(other.transform))
            enemyObject.nearbyColliders.Remove(other.transform);
    }
}