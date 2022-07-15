using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class WorldLoading : MonoBehaviour {
    [SerializeField] private WorldGen worldGen;
    [SerializeField] private Tilemap[] tilemaps;
    [SerializeField] private Follow cameraFollow;
    [SerializeField] private Vector2Int renderDistance;

    [HideInInspector] public World world;
    private Vector3Int currPos = Vector3Int.zero, prevPos = Vector3Int.zero;

    public bool InDungeon {
        get { return currPos.z == -1; }
        set { currPos.z = (value ? -1 : 0); }
    }

    RuleTile GetRuleTile(int x, int y, int layer) {
        return (InDungeon ? world.layers_dungeon : world.layers_overworld)[layer].Clamped(x, y);
    }

    public bool IsWater(int x, int y) {
        return worldGen.waterTiles.Contains(GetRuleTile(x, y, 0));
    }
    public bool IsDeepWater(int x, int y) {
        return worldGen.deepWaterTiles.Contains(GetRuleTile(x, y, 0));
    }

    public async void EnterExitDungeon(int enter) {
        FadeTransition.black = true;
        await FadeTransition.AwaitFade();
        InDungeon = (enter != 0);
        AdjustBounds();
        LoadAll();
        await General.NextFrame(10);
        FadeTransition.black = false;
    }

    public void AdjustBounds() {
        foreach(var tilemap in tilemaps) {
            tilemap.origin = new Vector3Int(currPos.x-renderDistance.x, currPos.y-renderDistance.y, 0);
            tilemap.size = new Vector3Int(renderDistance.x*2, renderDistance.y*2, 1);
            tilemap.ResizeBounds();
        }
    }

    void LoadAll() {
        for(int layer=0; layer<tilemaps.Length; ++layer) {
            var positionArray = new Vector3Int[tilemaps[layer].size.x*tilemaps[layer].size.y];
            var tilebaseArray = new RuleTile[positionArray.Length];

            Vector3Int pos = new Vector3Int(0, 0, 0);
            int i=0;
            for(pos.x=tilemaps[layer].origin.x; pos.x<tilemaps[layer].origin.x+tilemaps[layer].size.x; pos.x++) for(pos.y=tilemaps[layer].origin.y; pos.y<tilemaps[layer].origin.y+tilemaps[layer].size.y; pos.y++) {
                positionArray[i] = pos;
                tilebaseArray[i] = GetRuleTile(pos.x, pos.y, layer);
                ++i;
            }
            tilemaps[layer].SetTiles(positionArray, tilebaseArray);
            tilemaps[layer].RefreshAllTiles();
        }
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
        var tilebaseArray = new RuleTile[positionArray.Length];
        Vector3Int pos = new Vector3Int(0, 0, 0);
        for(int layer=0; layer<tilemaps.Length; ++layer) {
            int i=0;
            foreach(var bounds in toFill) {
                for(pos.x=bounds.min.x; pos.x<bounds.max.x; pos.x++) for(pos.y=bounds.min.y; pos.y<bounds.max.y; pos.y++) {
                    positionArray[i] = pos;
                    tilebaseArray[i] = GetRuleTile(pos.x, pos.y, layer);
                    ++i;
                }
            }
            tilemaps[layer].SetTiles(positionArray, tilebaseArray);
        }
    }

    void UpdatePos() {
        currPos.x = (int)Mathf.Floor(cameraFollow.transform.position.x);
        currPos.y = (int)Mathf.Floor(cameraFollow.transform.position.y);
    }

    void Update() {
        UpdatePos();

        if(currPos != prevPos) {
            AdjustBounds();
            LoadMissing();
        }

        prevPos.x = currPos.x;
        prevPos.y = currPos.y;
    }

    public void ForceLoadAllLagSpike() {
        UpdatePos();
        AdjustBounds();
        LoadAll();
    }
}