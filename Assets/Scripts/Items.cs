using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public struct ItemStats
{
    public string name;
    public Vector2Int size;
    public int maxStack;
    public bool equipable;
    public Sprite sprite;

    public ItemStats(string name, Vector2Int size, int maxStack, bool equipable)
    {
        this.name = name;
        this.size = size;
        this.maxStack = maxStack;
        this.equipable = equipable;
        this.sprite = Resources.Load<Sprite>("Sprites/item_" + name);
    }
}

public class Items 
{
    public static bool UseNone()
    {
        return true;
    }

    public static bool UseBlade()
    {
        PlayerStats.BeginMelee();
        return true;
    }

    public static bool UseBomb()
    {
        MonoBehaviour.Instantiate(PlayerStats.instance.prefab_bomb, PlayerStats.rigidbody.position, Quaternion.identity);
        PlayerStats.RemoveCurrentItem();
        return true;
    }

    public static bool UseMedkit()
    {
        PlayerStats.target.Heal(20);
        PlayerStats.RemoveCurrentItem();
        return true;
    }

    public static bool UseStimpack()
    {
        PlayerStats.target.Heal(10);
        PlayerStats.RemoveCurrentItem();
        return true;
    }

    public static bool UseCompass()
    {
        return true;
    }

    public static bool UsePotion()
    {
        PlayerStats.RemoveCurrentItem();
        return true;
    }

    public static bool UseFishingRod()
    {
        return true;
    }

    public static Dictionary<string, ItemStats> items = new Dictionary<string, ItemStats>();

    public static void Init()
    {
        
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