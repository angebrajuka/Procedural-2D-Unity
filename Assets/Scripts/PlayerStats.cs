using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // hierarchy
    public Transform prefab_bomb;
    public Transform knifeRotationPoint;
    public Transform knifeStart;
    public Transform weapons;
    public Transform entities;
    public float debugSpode;
    public Transform gunSpriteTransform;

    // components
    public static PlayerStats instance;
    public static Target target;
    public static PlayerHUD hud;
    public static Inventory inventory;
new public static Rigidbody2D rigidbody;
    public static PlayerAnimator playerAnimator;
    public static PlayerInput playerInput;
new public static Collider2D collider;


    // constant
    public const float k_RUN_ACCELL = 90.0f;
    public const float k_SPRINT_MULTIPLIER = 1.2f;
    public const float k_FLASHLIGHT_MULTIPLIER = 0.88f;
    public const float k_KNIFE_SPEED = 500f;
    public const float k_KNIFE_ARC = 70f;

    // upgrades + resources
    public static float g_KNIFE_DAMAGE=4;
    public static float energyMax, energy;

    // weapons
    public static float gunRpmTimer;
    public static float gunReloadTimer;
    public static GameObject reloadSound;

    // items
    public static bool melee;
    public static sbyte knifeDirection;
    public static string _currentItem = null;
    public static ItemStats currentItem = null;
    public static LinkedListNode<GridItem> currentItemNode;
    public static string interactItem = null;
    public static ItemPickup interactPickup;
    public static int interactPriority=0;

    // global
    public static byte difficulty; // 0 to 4
    public static byte save;
    public static bool load=false;
    public static bool loadingFirstChunks=true;


    public void Init()
    {
        instance = this;
        inventory = GetComponent<Inventory>();
        target = GetComponent<Target>();
        hud = GetComponent<PlayerHUD>();
        rigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerInput = GetComponent<PlayerInput>();
        collider = GetComponent<BoxCollider2D>();
    }

    public void Reset()
    {
        PauseHandler.Pause();
        PauseHandler.Blur();

        PlayerState.SwitchGun("", true);
        gunRpmTimer = 0;
        gunReloadTimer = 0;
        melee = false;
        energyMax = 50;
        ProceduralGeneration.reset = true;

        if(load)
        {
            Save_Load.Load(save);
        }
        else
        {
            rigidbody.position = ProceduralGeneration.center;
            
            while(entities.childCount > 0)
            {
                Transform child = entities.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            DynamicEnemySpawning.Reset();

            DaylightCycle.time = DaylightCycle.k_DAY*2f/3f;

            energy = energyMax;

            inventory.Clear();

            difficulty = 2;

            Save_Load.Save(save);
        }
        
        hud.UpdateHotbar();
    }

    public static void SubtractCurrentItem()
    {
        currentItemNode.Value.count --;
        if(currentItemNode.Value.count <= 0)
        {
            RemoveCurrentItem();
        } else
        {
            hud.UpdateHotbar();
        }
    }

    public static void RemoveCurrentItem()
    {
        Destroy(currentItemNode.Value.gameObject);
        currentItemNode.List.Remove(currentItemNode);
        currentItem = null;
        hud.UpdateHotbar();
    }

    public static void SetAmmo(int ammo)
    {
        if(currentItemNode != null) currentItemNode.Value.ammo = ammo;
    }

    public static int GetAmmo()
    {
        return currentItemNode == null ? 0 : currentItemNode.Value.ammo;
    }
}
