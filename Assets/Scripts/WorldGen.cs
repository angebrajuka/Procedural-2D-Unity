using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

[System.Serializable]
public struct MenuSeedPosition {
    public ushort seed;
    public float x, y;
}

public class WorldGen : MonoBehaviour {
    // hierarchy
    public Follow cameraFollow;
    public Tilemap tilemap;
    public GameObject prefab_decoration;
    public Transform decorPrefabs;
    public MenuSeedPosition[] menuSeeds;
    public Vector2Int renderDistance;
    public RuleTile[] dungeonTiles;

    public const int mapRadius = 800;
    public const int mapDiameter = mapRadius*2;
    public static readonly Vector2Int center = Vector2Int.one*mapRadius;

    private Vector3Int currPos = Vector3Int.zero;
    private Vector3Int prevPos = Vector3Int.zero;
    public Vector2 playerSpawnPoint = Vector2.zero;

    private RuleTile[] tiles;
    private HashSet<int> waterTiles;
    private Biome[] biomes;
    private const int rain_temp_map_width=100;
    private byte[,] rain_temp_map;

    private byte[,] mapTexture_biome, mapTexture_decor, mapTexture_dungeons, mapTexture_dungeonDecor;

    [HideInInspector] public Texture2D textureBiome, textureDecor, textureDungeons;

    [HideInInspector] public ushort seed;
    [HideInInspector] public float seed_main, seed_temp, seed_rain;

    public RuleTile LoadTile(string name) {
        return Resources.Load<RuleTile>("Tiles/"+name);
    }

    public void Clear() {
        textureBiome = null;
        textureDecor = null;
        textureDungeons = null;
        foreach(var pair in Biome.s_decorations) {
            if(pair.Value == null) continue;
            DestroyImmediate(pair.Value.gameObject);
        }
    }

    public void Start() {
        Biome.Init();

        var rain_temp_map_tex = Resources.Load<Texture2D>("BiomeData/rain_temp_map");
        var biomesJson = JsonUtility.FromJson<BiomesJson>(Resources.Load<TextAsset>("BiomeData/biomes").text).biomes;

        tiles = new RuleTile[biomesJson.Length];
        biomes = new Biome[biomesJson.Length];
        waterTiles = new HashSet<int>();
        rain_temp_map = new byte[rain_temp_map_width, rain_temp_map_width];

        for(int i=0; i<biomes.Length; i++) {
            tiles[i] = LoadTile(biomesJson[i].tile_name);
            if(biomesJson[i].tile_name.Equals("water_shallow") || biomesJson[i].tile_name.Equals("water")) {
                waterTiles.Add(i);
            }
            biomes[i] = new Biome(this, biomesJson[i]);
            if(biomesJson[i].rain_temp_map_color.Length == 3) {
                for(int x=0; x<rain_temp_map_width; x++) {
                    for(int y=0; y<rain_temp_map_width; y++) {
                        Color32 c = rain_temp_map_tex.GetPixel(x, y);

                        int[] arr = biomesJson[i].rain_temp_map_color;
                        if(c.r == arr[0] && c.g == arr[1] && c.b == arr[2]) {
                            rain_temp_map[x,y] = (byte)i;
                        }
                    }
                }
            }
        }

        mapTexture_biome = new byte[mapDiameter, mapDiameter];
        mapTexture_decor = new byte[mapDiameter, mapDiameter];
        mapTexture_dungeons = new byte[mapDiameter, mapDiameter];
        mapTexture_dungeonDecor = new byte[mapDiameter, mapDiameter];
    }

    public void GenerateTexture(int resolution) {
        var colors = new Color32[tiles.Length];
        for(int i=0; i<tiles.Length; i++) {
            colors[i] = tiles[i].m_DefaultSprite.texture.AverageColorFromTexture();
        }

        textureBiome = new Texture2D(resolution, resolution);
        for(int x=0; x<resolution; x++) {
            for(int y=0; y<resolution; y++) {
                textureBiome.SetPixel(x, y, colors[mapTexture_biome[(int)(((float)x/resolution)*mapDiameter), (int)(((float)y/resolution)*mapDiameter)]]);
            }
        }
        textureBiome.Apply();

        textureDecor = new Texture2D(resolution, resolution);
        for(int x=0; x<resolution; x++) {
            for(int y=0; y<resolution; y++) {
                textureDecor.SetPixel(x, y, mapTexture_decor[(int)(((float)x/resolution)*mapDiameter), (int)(((float)y/resolution)*mapDiameter)] < 254 ? Color.white : Color.black);
            }
        }
        textureDecor.Apply();

        textureDungeons = new Texture2D(resolution, resolution);
        for(int x=0; x<resolution; x++) {
            for(int y=0; y<resolution; y++) {
                textureDungeons.SetPixel(x, y, mapTexture_dungeons[(int)(((float)x/resolution)*mapDiameter), (int)(((float)y/resolution)*mapDiameter)] == 0 ? Color.black : Color.white);
            }
        }
        textureDungeons.Apply();
    }

    const int perlinOffset = 5429; // prevents mirroring

    // returns 0 for water, 1 for shallow water, 2 for sand, 3 for biome gen
    public byte PerlinMain(Vector2Int pos) {
        const float perlinScale = 0.004f;
        float perlinVal = Mathf.PerlinNoise((seed_main + pos.x + perlinOffset)*perlinScale, (seed_main + pos.y + perlinOffset)*perlinScale); // 0 to 1
        perlinVal = Math.Remap(perlinVal, 0, 1, 0.3f, 1);
        float gradientVal = 1-Vector2Int.Distance(pos, center)/(mapRadius); // 1 in center, 0 at edge of map
        float perlinScaleFine = 0.1f;
        float fineNoise = Mathf.PerlinNoise((seed_main + pos.x + perlinOffset)*perlinScaleFine, (seed_main + pos.y + perlinOffset)*perlinScaleFine);
        fineNoise = Math.Remap(fineNoise, 0, 1, 0, 0.05f);
        
        const float landVal = 0.52f;
        const float sandVal = 0.5f;
        const float shallowWaterVal = 0.48f;

        float val = (perlinVal+gradientVal)/2-fineNoise;
        return (byte)(val > landVal ? PerlinBiome(pos) : val > sandVal ? 2 : val > shallowWaterVal ? 1 : 0);
    }

    public byte PerlinBiome(Vector2Int pos) {
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

    void GenLand() {
        Vector2Int pos = new Vector2Int(0, 0);
        for(pos.x=0; pos.x<mapDiameter; pos.x++) {
            for(pos.y=0; pos.y<mapDiameter; pos.y++) {
                byte val = PerlinMain(pos);
                mapTexture_biome[pos.x, pos.y] = val;
            }
        }
    }

    void GenDecorations(System.Random rand) {
        Vector2Int pos = new Vector2Int(0, 0);
        for(pos.x=0; pos.x<mapDiameter; pos.x++) {
            for(pos.y=0; pos.y<mapDiameter; pos.y++) {
                mapTexture_decor[pos.x, pos.y] = 254;
            }
        }
        for(pos.x=0; pos.x<mapDiameter; pos.x++) {
            for(pos.y=0; pos.y<mapDiameter; pos.y++) {
                int val = mapTexture_biome[pos.x, pos.y];

                if(mapTexture_decor[pos.x, pos.y] == 254) {
                    var rval = rand.NextDouble();
                    for(int i=0; i<biomes[val].decorationThreshholds.Length; i++) {
                        if(rval < biomes[val].decorationThreshholds[i]) {
                            for(int x=0; x<biomes[val].decorations[i].stats.renderSize.x; x++) {
                                for(int y=0; y<biomes[val].decorations[i].stats.renderSize.y; y++) {
                                    if(mapTexture_biome.Clamped(pos.x+x, pos.y+y) != val ||
                                    (mapTexture_decor.Clamped(pos.x+x, pos.y+y) != 254 &&
                                    (mapTexture_decor.Clamped(pos.x+x, pos.y+y) == 255 ||
                                    biomes[val].decorations[mapTexture_decor.Clamped(pos.x+x, pos.y+y)].stats.collider != null))) {
                                        goto BreakBreak;
                                    }
                                }
                            }
                            for(int x=0; x<biomes[val].decorations[i].stats.size.x; x++) {
                                for(int y=0; y<biomes[val].decorations[i].stats.size.y; y++) {
                                    mapTexture_decor[pos.x+x, pos.y+y] = 255;
                                }
                            }
                            mapTexture_decor[pos.x, pos.y] = (byte)(i);
                            BreakBreak:
                            break;
                        }
                    }
                }
            }
        }
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

    void GenDungeons(System.Random rand) {
        int numDungeons = rand.Next(3, 6);

        var rooms = new List<Room>();

        var entrance = NewRoom(800, 800, 24, 18);

        rooms.Add(entrance);

        // for(int i=0; i<numDungeons; ++i) {

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
                mapTexture_dungeons[x, y] = 1;
            }
        }

        for(int x=0; x<mapDiameter; ++x) for(int y=0; y<mapDiameter; ++y) {
            mapTexture_dungeonDecor[x,y] = 254;
        }
    }

    void GenPlayerSpawn(System.Random rand) {
        var psp = rand.OnUnitCircle();
        playerSpawnPoint = center + psp*mapRadius;
        int jic;
        for(jic=0; jic < 999999 && GetTile((int)playerSpawnPoint.x, (int)playerSpawnPoint.y) <= 2; jic++) { // biome 0,1,2 is ocean,shoreline,beach
            playerSpawnPoint -= psp*6;
        }
        for(jic=0; jic<1000 && GetTile((int)playerSpawnPoint.x, (int)playerSpawnPoint.y) != 2; jic++) {
            playerSpawnPoint += psp;
        }
        playerSpawnPoint += rand.OnUnitCircle()*3;
    }

    public async Task GenerateMapAsync() {
        await Task.Run(()=> { GenerateMap(); });
    }

    public void GenerateMap() {
        var rand = new System.Random(seed);
        GenLand();
        GenDungeons(rand);
        GenDecorations(rand);
        GenPlayerSpawn(rand);
    }

    public async void EnterExitDungeon(int enter) {
        FadeTransition.black = true;
        await FadeTransition.AwaitFade();
        currPos.z = (enter == 0 ? 0 : -1);
        AdjustBounds();
        LoadAll();
        await General.NextFrame(10);
        FadeTransition.black = false;
    }

    public bool IsWater(int x, int y) {
        return currPos.z == -1 ? false : waterTiles.Contains(GetTile(x,y));
    }
    public bool IsOcean(int x, int y) {
        return currPos.z == -1 ? false : GetTile(x,y) == 0;
    }

    public int GetTile(int x, int y) {
        return (currPos.z == 0 ? mapTexture_biome : mapTexture_dungeons).Clamped(x, y);
    }

    public void SpawnDecoration(int x, int y, int tile, Transform parent) {
        int i = (currPos.z == 0 ? mapTexture_decor : mapTexture_dungeonDecor).Clamped(x, y);
        if(i == 254 || i == 255) return;

        var decoration = biomes[tile].decorations[i];
        var go = Instantiate(decoration.gameObject, new Vector3(x, y, 0), Quaternion.identity, parent);
        go.GetComponent<Decoration>().stats = decoration.stats; // not cloned because reference?
        go.SetActive(true);
    }

    public ushort RandomSeed() {
        return (ushort)Random.Range(0, 1000000);
    }

    public void SetSeed(ushort seed) {
        this.seed = seed;

        seed_main = 2589.216f+seed*252.3457f;
        seed_rain = 913.8473f+seed*2345.195f;
        seed_temp = 111.8325f+seed*762.0934f;
    }

    public void AdjustBounds() {
        var origin = tilemap.origin;
        origin.x = currPos.x - renderDistance.x;
        origin.y = currPos.y - renderDistance.y;
        tilemap.origin = origin;
        var size = tilemap.size;
        size.x = renderDistance.x*2;
        size.y = renderDistance.y*2;
        tilemap.size = size;
        tilemap.ResizeBounds();
    }

    TileBase GetTileBase(int x, int y) {
        return (currPos.z == 0 ? tiles : dungeonTiles)[GetTile(x, y)];
    }

    void LoadAll() {
        var positionArray = new Vector3Int[tilemap.size.x*tilemap.size.y];
        var tilebaseArray = new TileBase[positionArray.Length];

        Vector3Int pos=Vector3Int.zero;
        int i=0;
        for(pos.x=tilemap.origin.x; pos.x<tilemap.origin.x+tilemap.size.x; pos.x++) for(pos.y=tilemap.origin.y; pos.y<tilemap.origin.y+tilemap.size.y; pos.y++) {
            positionArray[i] = pos;
            tilebaseArray[i] = GetTileBase(pos.x, pos.y);
            ++i;
        }
        tilemap.SetTiles(positionArray, tilebaseArray);
        tilemap.RefreshAllTiles();
    }

    void LoadMissing() {
        Vector2Int diff = (Vector2Int)(currPos-prevPos);
        diff.x = Mathf.Abs(diff.x);
        diff.y = Mathf.Abs(diff.y);
        if(diff.x+diff.x >= renderDistance.x || diff.y+diff.y >= renderDistance.y) {
            LoadAll();
            return;
        }

        var toFill = new (Vector2Int min, Vector2Int max)[2];
        toFill[0].min = new Vector2Int(currPos.x > prevPos.x ? prevPos.x+renderDistance.x : currPos.x-renderDistance.x, currPos.y-renderDistance.y);
        toFill[0].max = new Vector2Int(currPos.x > prevPos.x ? currPos.x+renderDistance.x : prevPos.x-renderDistance.x, currPos.y+renderDistance.y);
        toFill[1].min = new Vector2Int(Mathf.Max(currPos.x, prevPos.x)-renderDistance.x, currPos.y > prevPos.y ? prevPos.y+renderDistance.y : currPos.y-renderDistance.y);
        toFill[1].max = new Vector2Int(Mathf.Min(currPos.x, prevPos.x)+renderDistance.x, currPos.y > prevPos.y ? currPos.y+renderDistance.y : prevPos.y-renderDistance.y);

        int area = 0;
        foreach(var bounds in toFill) {
            area += (bounds.max.x-bounds.min.x)*(bounds.max.y-bounds.min.y);
        }
        var positionArray = new Vector3Int[area];
        var tilebaseArray = new TileBase[positionArray.Length];
        Vector3Int pos=Vector3Int.zero;
        int i=0;
        foreach(var bounds in toFill) {
            for(pos.x=bounds.min.x; pos.x<bounds.max.x; pos.x++) for(pos.y=bounds.min.y; pos.y<bounds.max.y; pos.y++) {
                positionArray[i] = pos;
                tilebaseArray[i] = GetTileBase(pos.x, pos.y);
                ++i;
            }
        }
        tilemap.SetTiles(positionArray, tilebaseArray);
    }

    void UpdatePos() {
        currPos.x = (int)Mathf.Floor(cameraFollow.transform.position.x);
        currPos.y = (int)Mathf.Floor(cameraFollow.transform.position.y);
        // currPos.z gets manually changed on staircase or smth
    }

    void Update() {
        UpdatePos();

        if(currPos != prevPos) {
            AdjustBounds();
            LoadMissing();
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;
        prevPos.z = currPos.z;
    }

    public void ForceLoadAllLagSpike() {
        UpdatePos();
        AdjustBounds();
        LoadAll();
    }
}
