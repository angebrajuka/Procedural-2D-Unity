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
    public GameObject[] prefabs;

    public ItemStats(JsonItem jsonItem)
    {
        name = jsonItem.name;
        maxStack = jsonItem.maxStack;
        equipable = jsonItem.equipable;
        sprite = Resources.Load<Sprite>("Sprites/Items/"+name);
        size = new Vector2Int((int)(sprite.texture.width/8f), (int)(sprite.texture.height/8f));
        gun = Items.guns.ContainsKey(name) ? Items.guns[name] : null;

        prefabs = new GameObject[5];
        for(int i=0; true; i++)
        {
            prefabs[i] = Resources.Load<GameObject>("ItemData/prefab_"+name+"_"+i);
            if(prefabs[i] == null)
            {
                Array.Resize(ref prefabs, i);
                break;
            }
            prefabs[i].SetActive(false);
            Array.Resize(ref prefabs, prefabs.Length*2);
        }
    }

    public void Use()
    {
        switch(name)
        {
        case "bomb":
            MonoBehaviour.Instantiate(prefabs[0], PlayerMovement.rb.position, Quaternion.identity, Entities.t).SetActive(true);
            PlayerStats.SubtractCurrentItem();
            break;
        case "blade":
            PlayerState.BeginMelee();
            break;
        case "medkit":
            PlayerTarget.target.Heal(20);
            PlayerStats.SubtractCurrentItem();
            break;
        case "stimpack":
            PlayerTarget.target.Heal(10);
            PlayerStats.SubtractCurrentItem();
            break;
        case "potion":
            PlayerStats.RemoveCurrentItem();
            break;
        case "fishing_rod":
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
    public string   bulletTrailPrefab;
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

    public static string[] GetAmmoTypes()
    {
        var types = new HashSet<string>();
        foreach(var pair in guns)
        {
            if(!types.Contains(pair.Value.ammoType))
            {
                types.Add(pair.Value.ammoType);
            }
        }
        var arr = new string[types.Count];
        int i = 0;
        foreach(var str in types)
        {
            arr[i] = str;
            i++;
        }
        return arr;
    }
}