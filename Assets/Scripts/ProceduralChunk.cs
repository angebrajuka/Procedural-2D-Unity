using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralChunk : MonoBehaviour
{
    public enum State : byte
    {
        LOADING,
        LOADED,
        UNLOADING
    }

    public Vector2Int pos;
    public State state;

    static readonly Vector2Int center = Vector2Int.one*ProceduralGeneration.mapRadius;

    float Perlin(Vector2Int pos)
    {
        return Mathf.PerlinNoise(ProceduralGeneration.seed + pos.x, ProceduralGeneration.seed + pos.y) - Vector2Int.Distance(pos, center)/ProceduralGeneration.mapRadius;
    }

    void Update()
    {
        switch(state)
        {
        case State.LOADING:



            state = State.LOADED;
            ProceduralGeneration.instance.CheckLoaded();
            break;
        case State.UNLOADING:

            break;
        default:
            break;
        }
    }
}
