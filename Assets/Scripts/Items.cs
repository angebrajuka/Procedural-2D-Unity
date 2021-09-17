using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable] 
public class JsonItem
{
    public string name;
    public int maxStack;
    public bool equipable;
}

[System.Serializable] 
public class ItemsJson
{
    public JsonItem[] items;
}

public class ItemStats
{
    public string name;
    public Vector2Int size;
    public int maxStack;
    public bool equipable;
    public Sprite sprite;
    public Gun gun;

    public ItemStats(JsonItem jsonItem)
    {
        name = jsonItem.name;
        maxStack = jsonItem.maxStack;
        equipable = jsonItem.equipable;
        sprite = Resources.Load<Sprite>("Sprites/Items/"+name);
        size = new Vector2Int((int)(sprite.texture.width/16f), (int)(sprite.texture.height/16f));
        gun = Items.guns.ContainsKey(name) ? Items.guns[name] : null;
    }

    public void Use()
    {
        switch(name)
        {
        case "bomb":
            MonoBehaviour.Instantiate(PlayerStats.instance.prefab_bomb, PlayerStats.rigidbody.position, Quaternion.identity);
            PlayerStats.RemoveCurrentItem();
            break;
        case "Blade":
            PlayerStats.BeginMelee();
            break;
        case "Medkit":
            PlayerStats.target.Heal(20);
            PlayerStats.RemoveCurrentItem();
            break;
        case "Stimpack":
            PlayerStats.target.Heal(10);
            PlayerStats.RemoveCurrentItem();
            break;
        case "Potion":
            PlayerStats.RemoveCurrentItem();
            break;
        case "FishingRod":
            break;
        default:
            break;
        }
    }
}

[System.Serializable]
public class GunsJson
{
    public JsonGun[] guns;
}

[System.Serializable]
public class JsonGun
{
    public string   name;
    public float    damage;
    public string   ammoType;
    public float    rpm;
    public float    spread;
    public float    range;
    public int      clipSize;
    public int      ammoPerShot;
    public float    reloadTime;
    public float    recoil;
    public int      pellets;
    public float    volume_shoot;
    public float    volume_reload;
    public int      muzzleFlashPrefab;
    public float[]  barrelTip;
}

public class Items 
{
    public static Dictionary<string, ItemStats> items;
    public static Dictionary<string, Gun> guns;

    public static void Init(Transform gunSpriteTransform)
    {
        var gunsJson = JsonUtility.FromJson<GunsJson>(Resources.Load<TextAsset>("ItemData/guns").text).guns;
        guns = new Dictionary<string, Gun>();
        items = new Dictionary<string, ItemStats>();
        foreach(var jsonGun in gunsJson)
        {
            var gun = new Gun(jsonGun, gunSpriteTransform);
            guns.Add(jsonGun.name, gun);
            var item = new JsonItem();
            item.name = gun.name;
            item.maxStack = 1;
            item.equipable = true;
            items.Add(item.name, new ItemStats(item));
        }

        var itemsJson = JsonUtility.FromJson<ItemsJson>(Resources.Load<TextAsset>("ItemData/items").text).items;
        foreach(var jsonItem in itemsJson)
        {
            items.Add(jsonItem.name, new ItemStats(jsonItem));
        }
    }

    // {
        // new ItemStats("NONE",           new Vector2Int(0, 0),   0,  false),
        // new ItemStats("BLADE",          new Vector2Int(2, 1),   1,  true),
        // new ItemStats("BOMB",           new Vector2Int(2, 2),   1,  true),
        // new ItemStats("MEDKIT",         new Vector2Int(3, 2),   1,  true),
        // new ItemStats("STIMPACK",       new Vector2Int(1, 1),   1,  true),
        // new ItemStats("COMPASS",        new Vector2Int(2, 2),   1,  true),
        // new ItemStats("POTION",         new Vector2Int(2, 3),   1,  true),
        // new ItemStats("FISHING_ROD",    new Vector2Int(3, 3),   1,  true),

        // new ItemStats("BULLETS_SMALL",  new Vector2Int(1, 1),   30, false),
        // new ItemStats("BULLETS_LARGE",  new Vector2Int(1, 1),   20, false),
        // new ItemStats("SHELLS",         new Vector2Int(1, 1),   16, false),
        // new ItemStats("PLASMA",         new Vector2Int(1, 1),   60, false),


        // new ItemStats("PISTOL",         new Vector2Int(3, 2),   0,  1,  true,   UseNone),
        // new ItemStats("SMG",            new Vector2Int(4, 3),   1,  1,  true,   UseNone),
        // new ItemStats("ASSAULT_RIFLE",  new Vector2Int(6, 3),   2,  1,  true,   UseNone),
        // new ItemStats("DMR",            new Vector2Int(7, 3),   3,  1,  true,   UseNone),
        // new ItemStats("SHOTGUN_PUMP",   new Vector2Int(6, 3),   4,  1,  true,   UseNone),
        // new ItemStats("SHOTGUN_DOUBLE", new Vector2Int(5, 3),   5,  1,  true,   UseNone),
        // new ItemStats("SHOTGUN_AUTO",   new Vector2Int(6, 3),   6,  1,  true,   UseNone),
    // };
}