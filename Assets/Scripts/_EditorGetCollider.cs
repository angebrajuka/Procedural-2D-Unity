using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _EditorGetCollider : MonoBehaviour
{
    public PolygonCollider2D c;
    public SpriteRenderer sr;
    public string output;

    public void Get()
    {
        output = sr.sprite.texture.name+": ";
        foreach(var vec in c.points)
        {
            output += (vec.ToString().Replace(" ", "")+", ").Replace("(", "").Replace(")", "");
        }
        output = output.Remove(output.Length - 2);
    }
}
