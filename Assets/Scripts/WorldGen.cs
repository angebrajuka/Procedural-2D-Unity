﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

public class WorldGen : MonoBehaviour {
    [SerializeField] private RuleTile tile_dungeonFloor;
    [SerializeField] private RuleTile tile_dungeonWall;
    [SerializeField] private RuleTile[] water;
    [SerializeField] private RuleTile[] deep_water;
    [SerializeField] private Biome ocean, shoreline, beach;
    [SerializeField] private Biome[] biomes;

    public HashSet<RuleTile> waterTiles;
    public HashSet<RuleTile> deepWaterTiles;
    private const int rain_temp_map_width=100;
    private Biome[,] rain_temp_map;

    public void Start() {
        var rain_temp_map_tex = Resources.Load<Texture2D>("BiomeData/rain_temp_map");

        waterTiles = new HashSet<RuleTile>();
        deepWaterTiles = new HashSet<RuleTile>();
        rain_temp_map = new Biome[rain_temp_map_width, rain_temp_map_width];

        foreach(var tile in water) {
            waterTiles.Add(tile);
        }
        foreach(var tile in deep_water) {
            deepWaterTiles.Add(tile);
        }

        foreach(var biome in biomes) {
            biome.Init();
            for(int x=0; x<rain_temp_map_width; x++) for(int y=0; y<rain_temp_map_width; y++) {
                if(rain_temp_map_tex.GetPixel(x, y) == (Color)biome.rain_temp_map_color) {
                    rain_temp_map[x,y] = biome;
                }
            }
        }
    }

    public ushort RandomSeed() {
        return (ushort)Random.Range(0, 1000000);
    }





    const int perlinOffset = 5429; // prevents mirroring

    public Biome PerlinBiome(Vector2Int pos, float seed_main, float seed_rain, float seed_temp) {
        const float perlinScaleRain = 0.003f;
        const float perlinScaleTemp = 0.003f;

        float perlinValRain = Mathf.PerlinNoise((seed_rain + pos.x + perlinOffset)*perlinScaleRain, (seed_rain + pos.y + perlinOffset)*perlinScaleRain);
        float perlinValTemp = Mathf.PerlinNoise((seed_temp + pos.x + perlinOffset)*perlinScaleTemp, (seed_temp + pos.y + perlinOffset)*perlinScaleTemp);

        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((seed_main + pos.x + perlinOffset)*perlinScaleFine, (seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);

        perlinValTemp -= fineNoise;
        perlinValTemp = Mathf.Round(perlinValTemp * rain_temp_map_width);
        perlinValRain -= fineNoise;
        perlinValRain = Mathf.Round(perlinValRain * rain_temp_map_width);

        return rain_temp_map.Clamped((int)perlinValTemp, (int)perlinValRain);
    }

    public Biome PerlinMain(Vector2Int pos, float seed_main, float seed_rain, float seed_temp) {
        const float perlinScale = 0.004f;
        float perlinVal = Mathf.PerlinNoise((seed_main + pos.x + perlinOffset)*perlinScale, (seed_main + pos.y + perlinOffset)*perlinScale); // 0 to 1
        perlinVal = Math.Remap(perlinVal, 0, 1, 0.3f, 1);
        float gradientVal = 1-Vector2Int.Distance(pos, World.center)/(World.radius); // 1 in center, 0 at edge of map
        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((seed_main + pos.x + perlinOffset)*perlinScaleFine, (seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);
        
        const float landVal = 0.52f;
        const float sandVal = 0.5f;
        const float shallowWaterVal = 0.48f;

        float val = (perlinVal+gradientVal)/2-fineNoise;
        return (val > landVal ? PerlinBiome(pos, seed_main, seed_rain, seed_temp) : val > sandVal ? beach : val > shallowWaterVal ? shoreline : ocean);
    }

    RuleTile[,] GenLand(float seed_main, float seed_rain, float seed_temp) {
        var layer = new RuleTile[World.diameter, World.diameter];
        Vector2Int pos = new Vector2Int(0, 0);
        for(pos.x=0; pos.x<World.diameter; pos.x++) {
            for(pos.y=0; pos.y<World.diameter; pos.y++) {
                layer[pos.x, pos.y] = PerlinMain(pos, seed_main, seed_rain, seed_temp).tile;
            }
        }
        return layer;
    }

    RuleTile[,] GenDecorations(System.Random rand, float seed_main, float seed_rain, float seed_temp) {
        var layer = new RuleTile[World.diameter, World.diameter];
        var filled = new bool[World.diameter, World.diameter];
        Vector2Int pos = new Vector2Int(0, 0);
        for(pos.x=0; pos.x<World.diameter; pos.x++) {
            for(pos.y=0; pos.y<World.diameter; pos.y++) {
                layer[pos.x, pos.y] = null;
                filled[pos.x, pos.y] = false;
            }
        }
        for(pos.x=0; pos.x<World.diameter; pos.x++) {
            for(pos.y=0; pos.y<World.diameter; pos.y++) {
                Biome biome = PerlinMain(pos, seed_main, seed_rain, seed_temp);

                if(layer[pos.x, pos.y] == null) {
                    var rval = rand.NextDouble();
                    foreach(var decoration in biome.decorations) {
                        if(rval < decoration.threshhold) {

                            bool DecorationFits() {
                                for(int x=0; x<decoration.Size.x; x++) {
                                    for(int y=0; y<decoration.Size.y; y++) {
                                        if(PerlinMain(pos+new Vector2Int(x, y), seed_main, seed_rain, seed_temp) != biome || filled[pos.x+x, pos.y+y]) {
                                            return false;
                                        }
                                    }
                                }
                                return true;
                            }

                            if(DecorationFits()) {
                                if(decoration.collider) {
                                    for(int x=0; x<decoration.Size.x; x++) {
                                        for(int y=0; y<decoration.Size.y; y++) {
                                            filled[pos.x+x, pos.y+y] = true;
                                        }
                                    }
                                }
                                layer[pos.x, pos.y] = decoration.tile;
                                break;
                            }
                        }
                    }
                }
            }
        }

        return layer;
    }

    public struct Room {
        public Vector2Int BL, TR;
        public int up, down, left, right;

        public Vector2Int Center { get { return (BL+TR)/2; } }
        public int Width  { get { return TR.x-BL.x+1; } }
        public int Height { get { return TR.y-BL.y+1; } }
    };

    Room NewRoom(int x, int y, int w, int h) {
        var room = new Room();
        room.BL = new Vector2Int(x, y);
        room.TR = new Vector2Int(x+w, y+h);
        return room;
    }

    RuleTile[][,] GenDungeons(System.Random rand) {
        var floor = new RuleTile[World.diameter, World.diameter];
        var decor = new RuleTile[World.diameter, World.diameter];
        Vector2Int pos = new Vector2Int(0, 0);
        for(pos.x=0; pos.x<World.diameter; pos.x++) {
            for(pos.y=0; pos.y<World.diameter; pos.y++) {
                floor[pos.x, pos.y] = null;
                decor[pos.x, pos.y] = tile_dungeonWall;
            }
        }

        var rooms = new List<Room>();

        var entrance = NewRoom(800, 800, 24, 18);

        rooms.Add(entrance);

        // for(int i=0; i<9; ++i) {

        // add to mapTexture_decor

        

        // Vector2Int TL=entrance, BR;
        // TL.Add(-rand.Next(3, 10), -rand.Next(2, 7));
        // BR = TL;
        // BR.Add(24, 18);

        // while(something) add side rooms

        // generate alternative enterances/exits

        // populate rooms

        // }

        foreach(var room in rooms) {
            for(int x=room.BL.x; x<=room.TR.x; ++x) for(int y=room.BL.y; y<=room.TR.y; ++y) {
                floor[x,y] = tile_dungeonFloor;
                decor[x,y] = null;
            }
        }

        return new RuleTile[][,]{floor, decor};
    }

    Vector2 GenPlayerSpawn(System.Random rand, World world) {
        var psp = rand.OnUnitCircle();
        var playerSpawnPoint = World.center + psp*World.radius;
        int jic;
        RuleTile GetTile() {
            return world.layers_overworld[0][(int)playerSpawnPoint.x, (int)playerSpawnPoint.y];
        }
        for(jic=0; jic < 999999; jic++) { // move in until land
            playerSpawnPoint -= psp*6;
            RuleTile tile = GetTile();
            if(tile != ocean.tile && tile != shoreline.tile) break;
        }
        for(jic=0; jic<1000 && GetTile() != beach.tile; jic++) { // move out until beach
            playerSpawnPoint += psp;
        }
        playerSpawnPoint += rand.OnUnitCircle()*3;
        return playerSpawnPoint;
    }

    public World GenerateWorld(ushort seed) {
        World world = new World();
        world.seed = seed;
        world.seed_main = 2589.216f+seed*252.3457f;
        world.seed_rain = 913.8473f+seed*2345.195f;
        world.seed_temp = 111.8325f+seed*762.0934f;
        world.layers_overworld = new RuleTile[2][,];
        var rand = new System.Random(seed);
        world.layers_overworld[0] = GenLand(world.seed_main, world.seed_rain, world.seed_temp);
        world.layers_dungeon = GenDungeons(rand);
        world.layers_overworld[1] = GenDecorations(rand, world.seed_main, world.seed_rain, world.seed_temp);
        world.playerSpawnPoint = GenPlayerSpawn(rand, world);
        return world;
    }




    public (Texture2D[] textures_overworld, Texture2D[] textures_dungeons) GenerateTextures(int resolution, World world) {
        Texture2D[] textures_overworld = new Texture2D[world.layers_overworld.Length],
                    textures_dungeons = new Texture2D[world.layers_dungeon.Length];

        var tile_to_color = new Dictionary<RuleTile, Color32>();

        var textures = textures_overworld;
        var layers = world.layers_overworld;
        while(true) {
            for(int i=0; i<textures.Length; ++i) {
                textures[i] = new Texture2D(resolution, resolution);
                for(int x=0; x<resolution; x++) for(int y=0; y<resolution; y++) {
                    var tile = layers[i][layers[i].GetLength(0)*x/resolution, layers[i].GetLength(1)*y/resolution];
                    if(tile == null) {
                        textures[i].SetPixel(x, y, Color.white);
                        continue;
                    }
                    if(!tile_to_color.ContainsKey(tile)) {
                        tile_to_color.Add(tile, tile.m_DefaultSprite.texture.AverageColorFromTexture());
                    }
                    textures[i].SetPixel(x, y, tile_to_color[tile]);
                }
                textures[i].Apply();
            }
            if(textures == textures_dungeons) break;
            textures = textures_dungeons;
            layers = world.layers_dungeon;
        }

        return (textures_overworld, textures_dungeons);
    }
}
