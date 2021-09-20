using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public bool scale;

    public void Init(Vector3 vec)
    {
        if(scale)
        {
            transform.localScale += vec;
        }
    }
}