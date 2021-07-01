using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachOnLoad : MonoBehaviour
{
    void Start()
    {
        transform.parent = null;
    }
}
