using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math
{
    public static float ROOT_ONE_HALF = 0.70710678118654752440084436210485f;
    
    public static float VecToAngle(Vector2 vec)
    {
        vec.Normalize();
        return NormalizedVecToAngle(vec);
    }

    public static float NormalizedVecToAngle(Vector2 vec)
    {
        float angle = Vector3.Angle(Vector3.right, vec);
        if(vec.y < 0) angle = 360-angle;
        return angle;
    }

    public static readonly byte[,] directions4 =
    {
        {2, 1, 0}, 
        {2, 3, 0}, 
        {2, 3, 0}
    };
    
    public static readonly Vector2[] vectors =
    {
        Vector2.right,
        new Vector2(ROOT_ONE_HALF, ROOT_ONE_HALF),
        Vector2.up,
        new Vector2(-ROOT_ONE_HALF, ROOT_ONE_HALF),
        Vector2.left,
        new Vector2(-ROOT_ONE_HALF, -ROOT_ONE_HALF),
        Vector2.down,
        new Vector2(ROOT_ONE_HALF, -ROOT_ONE_HALF)
    };
    
    public static int AngleToDir4(float angle)
    {
        angle /= 45;
        if(angle < 0) angle = 0;
        return (int)Mathf.Round(angle);
    }

    public static bool Closer(Vector2 pos1, Vector2 pos2, float distance)
    {
        float dx = pos2.x-pos1.x;
        float dy = pos2.y-pos2.y;
        return dx*dx+dy*dy < distance*distance;
    }
}
