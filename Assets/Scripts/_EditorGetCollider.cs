using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _EditorGetCollider : MonoBehaviour
{
    public PolygonCollider2D c;
    public string output;

    public void Get()
    {
        output = "";
        foreach(var vec in c.points)
        {
            output += (vec.ToString().Replace(" ", "")+"; ").Replace("(", "").Replace(")", "");
        }
        output = output.Remove(output.Length - 2);
    }
}
