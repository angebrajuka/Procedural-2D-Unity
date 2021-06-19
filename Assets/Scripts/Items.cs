using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum Item:byte {
    NONE,
    BLADE,
    BOMB,
    MEDKIT,
    STIMPACK,
    COMPASS,
    POTION,
    FISHING_ROD,
    FLASHLIGHT,

    PISTOL,
    SMG,
    ASSAULT_RIFLE,
    DMR,
    SHOTGUN_PUMP,
    SHOTGUN_DOUBLE,
    SHOTGUN_AUTO,
    ENERGY_RIFLE,
    ENERGY_RAILGUN
}

public struct ItemStats {
    public string name;
    public Vector2Int size;
    public sbyte gun;
    public Func<bool> use;
    public Sprite sprite;

    public ItemStats(string name, Vector2Int size, sbyte gun, Func<bool> use) {
        this.name = name;
        this.size = size;
        this.gun = gun;
        this.use = use;
        this.sprite = Resources.Load<Sprite>((gun == -1 ? "UI/item_" : "Sprites/gun_") + name);
    }
}

public class Items {

    public static bool UseNone() {
        return true;
    }

    public static bool UseBlade() {
        PlayerStats.BeginMelee();
        return true;
    }

    public static bool UseBomb() {
        MonoBehaviour.Instantiate(PlayerStats.playerStats.prefab_bomb, PlayerStats.rigidbody.position, Quaternion.identity);
        PlayerStats.RemoveCurrentItem();
        return true;
    }

    public static bool UseMedkit() {
        PlayerStats.target.Heal(20);
        PlayerStats.RemoveCurrentItem();
        return true;
    }

    public static bool UseStimpack() {
        PlayerStats.target.Heal(10);
        PlayerStats.RemoveCurrentItem();
        return true;
    }

    public static bool UseCompass() {
        return true;
    }

    public static bool UsePotion() {
        PlayerStats.RemoveCurrentItem();
        return true;
    }

    public static bool UseFishingRod() {
        return true;
    }

    public static readonly ItemStats[] items = new ItemStats[] {
        new ItemStats("NONE",           new Vector2Int(0, 0),   -1, UseNone),
        new ItemStats("BLADE",          new Vector2Int(2, 1),   -1, UseBlade),
        new ItemStats("BOMB",           new Vector2Int(2, 2),   -1, UseBomb),
        new ItemStats("MEDKIT",         new Vector2Int(3, 2),   -1, UseMedkit),
        new ItemStats("STIMPACK",       new Vector2Int(1, 1),   -1, UseStimpack),
        new ItemStats("COMPASS",        new Vector2Int(2, 2),   -1, UseCompass),
        new ItemStats("POTION",         new Vector2Int(2, 3),   -1, UsePotion),
        new ItemStats("FISHING_ROD",    new Vector2Int(3, 3),   -1, UseFishingRod),
        new ItemStats("FLASHLIGHT",     new Vector2Int(2, 1),   -1, UseNone),
        new ItemStats("PISTOL",         new Vector2Int(3, 2),   0,  UseNone),
        new ItemStats("SMG",            new Vector2Int(5, 3),   1,  UseNone),
        new ItemStats("ASSAULT_RIFLE",  new Vector2Int(6, 3),   2,  UseNone),
        new ItemStats("DMR",            new Vector2Int(7, 3),   3,  UseNone),
        new ItemStats("SHOTGUN_PUMP",   new Vector2Int(6, 3),   4,  UseNone),
        new ItemStats("SHOTGUN_DOUBLE", new Vector2Int(5, 3),   5,  UseNone),
        new ItemStats("SHOTGUN_AUTO",   new Vector2Int(6, 3),   6,  UseNone),
        new ItemStats("ENERGY_RIFLE",   new Vector2Int(6, 3),   7,  UseNone),
        new ItemStats("ENERGY_RAILGUN", new Vector2Int(7, 3),   8,  UseNone),
    };
}