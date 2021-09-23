using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public static Prefabs instance;

    // hierarchy

    public void Init()
    {
        instance = this;
    }
}