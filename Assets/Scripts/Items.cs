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
            MonoBehaviour.Instantiate(PlayerStats.instance.prefab_bomb, PlayerMovement.rb.position, Quaternion.identity);
            PlayerStats.SubtractCurrentItem();
            break;
        case "Blade":
            PlayerState.BeginMelee();
            break;
        case "Medkit":
            PlayerTarget.target.Heal(20);
            PlayerStats.RemoveCurrentItem();
            break;
        case "Stimpack":
            PlayerTarget.target.Heal(10);
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
    public string   muzzleFlashPrefab;
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
}