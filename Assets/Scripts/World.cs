using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct World {
    public const int radius = 800;
    public const int diameter = radius*2;
    public static readonly Vector2Int center = Vector2Int.one*radius;

    public ushort seed;
    public float seed_main, seed_temp, seed_rain;
    public RuleTile[][,] layers_overworld;
    public RuleTile[][,] layers_dungeon;
    public Vector2 playerSpawnPoint;
}