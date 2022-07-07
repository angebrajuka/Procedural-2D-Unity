using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Singles;

[System.Serializable]
public class StringFloat
{
    public string s;
    public float f;
}

[System.Serializable]
public class JsonDecoration
{
    public string name;
    public StringFloat[] item_drops;
    public int health;
    public float[] collider;
}

[System.Serializable]
public class DecorationsJson
{
    public JsonDecoration[] decorations;
    public StringFloat[] alt_sorting_layers;
}

[System.Serializable]
public class JsonBiome
{
    public string biome_name;
    public string tile_name;
    public int[] rain_temp_map_color;
    public StringFloat[] decorations;
}

[System.Serializable]
public class BiomesJson
{
    public JsonBiome[] biomes;
}

public class DecorationStats
{
    public int health;
    public Sprite sprite;
    public Vector2Int size;
    public Vector2Int renderSize;
    public Vector2[] collider;
    public StringFloat[] itemDrops;
    public int sortingLayer;

    static readonly Vector2Int one_two = new Vector2Int(1, 2);

    public DecorationStats(string name, int health, Vector2[] collider, StringFloat[] itemDrops, int sortingLayer)
    {
        this.health = health;
        this.sprite = Resources.Load<Sprite>("Sprites/Decorations/"+name);
        this.renderSize = new Vector2Int((int)sprite.rect.width/(int)sprite.pixelsPerUnit, (int)sprite.rect.height/(int)sprite.pixelsPerUnit);
        this.size = Math.Divide(renderSize, one_two);
        this.collider = collider;
        this.itemDrops = itemDrops;
        for(int i=0; i<itemDrops.Length; i++)
        {
            itemDrops[i].f += i > 0 ? itemDrops[i-1].f : 0;
        }
        this.sortingLayer = sortingLayer;
    }

    public static DecorationStats Default(string name)
    {
        return new DecorationStats(name, 1, null, new StringFloat[0], 1);
    }
}

public struct Biome
{
    public static Dictionary<string, DecorationStats> s_decorationStats = new Dictionary<string, DecorationStats>();
    public static Dictionary<string, Decoration> s_decorations = new Dictionary<string, Decoration>();

    public static void Init()
    {
        var decorationsJson = JsonUtility.FromJson<DecorationsJson>(Resources.Load<TextAsset>("BiomeData/decorations").text);
        
        var sortingLayers = new Dictionary<string, int>();
        foreach(var layer in decorationsJson.alt_sorting_layers)
        {
            sortingLayers.Add(layer.s, (int)layer.f);
        }

        var decorations = decorationsJson.decorations;

        s_decorationStats.Clear();
        for(int i=0; i<decorations.Length; i++)
        {
            string name = decorations[i].name;

            var collider = new Vector2[decorations[i].collider.Length/2];
            for(int p=0; p<collider.Length; p++)
            {
                collider[p] = new Vector2(decorations[i].collider[2*p], decorations[i].collider[2*p+1]);
            }

            s_decorationStats.Add(name, new DecorationStats(
                name,
                decorations[i].health,
                collider,
                decorations[i].item_drops,
                sortingLayers.ContainsKey(name) ? sortingLayers[name] : 1
            ));
        }
    }

    public Decoration[] decorations;
    public float[] decorationThreshholds;

    public Biome(WorldGen worldGen, JsonBiome jsonBiome)
    {
        decorations = new Decoration[jsonBiome.decorations.Length];
        decorationThreshholds = new float[jsonBiome.decorations.Length];

        for(int i=0; i<decorations.Length; i++)
        {
            var name = jsonBiome.decorations[i].s;

            if(!s_decorationStats.ContainsKey(name))
            {
                s_decorationStats.Add(name, DecorationStats.Default(name));
            }

            if(!s_decorations.ContainsKey(name))
            {
                GameObject go = MonoBehaviour.Instantiate(worldGen.prefab_decoration, worldGen.decorPrefabs);
                go.name = name;
                s_decorations.Add(name, go.GetComponent<Decoration>());
                s_decorations[name].Init(s_decorationStats[name]);
                go.SetActive(false);
            }

            decorations[i] = s_decorations[name];
            decorationThreshholds[i] = jsonBiome.decorations[i].f;
            decorationThreshholds[i] += (i > 0 ? decorationThreshholds[i-1] : 0);
        }
    }
}