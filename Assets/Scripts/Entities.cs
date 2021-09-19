using UnityEngine;

public class Entities : MonoBehaviour
{
    public static Transform t;

    public void Init()
    {
        t = transform;
    }

    public static void Clear()
    {
        while(t.childCount > 0)
        {
            Transform child = t.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }
}