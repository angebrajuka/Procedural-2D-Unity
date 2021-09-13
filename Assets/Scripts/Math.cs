using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math
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

    public static readonly byte[,] directions =
    {
        {1, 0, 0}, 
        {1, 0, 0}, 
        {1, 1, 0}
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
    
    public static int AngleToDir(float angle)
    {
        angle %= 360;
        return (angle >= 90 && angle <= 270) ? 1 : 0;
    }

    public static bool Closer(Vector2 pos1, Vector2 pos2, float distance)
    {
        float dx = pos2.x-pos1.x;
        float dy = pos2.y-pos2.y;
        return dx*dx+dy*dy < distance*distance;
    }

    public static float Sqr(float x)
    {
        return x*x;
    }

    public static float Dist(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt(Sqr(x2-x1)+Sqr(y2-y1));
    }

    public static Vector3Int Vec3(Vector2Int vec)
    {
        return new Vector3Int(vec.x, vec.y, 0);
    }

    public static Vector3 Vec3(Vector2 vec)
    {
        return new Vector3(vec.x, vec.y, 0);
    }

    public static Vector2 Vec2(Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static bool Ish(float value1, float value2, float range=0.05f)
    {
        return Mathf.Abs(value1-value2) < range;
    }
}
