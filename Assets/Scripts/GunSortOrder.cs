using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSortOrder : MonoBehaviour
{
    // inspector
    public RectTransform sortPoint;
    public RectTransform gunSprite;

    // components
    [HideInInspector] public RectTransform rect;
    Vector3[] corners;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        corners = new Vector3[4];
    }

    static readonly Vector2[] pivots = new Vector2[]{
        new Vector2(0, 0),
        new Vector2(0, -0.6f),
        new Vector2(-1.4f, -0.6f),
        new Vector2(-1.4f, 0)
    };

    void Update()
    {
        rect.GetWorldCorners(corners);
        
        float a = 0;//PlayerInput.angle;
        int corner =    a > 0 && a < 90 ? 0 :       // top right
                        a >= 90 && a < 180 ? 1 :    // top left
                        a >= 180 && a < 270 ? 2 :   // bottom left
                        3;                          // bottom right
        sortPoint.position = corners[corner];
        gunSprite.anchoredPosition = pivots[corner];
    }
}