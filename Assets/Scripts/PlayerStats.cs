using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerState;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    // constant
    public const float k_RUN_ACCELL = 90.0f;
    public const float k_SPRINT_MULTIPLIER = 1.2f;
    public const float k_FLASHLIGHT_MULTIPLIER = 0.88f;
    public const float k_KNIFE_SPEED = 500f;
    public const float k_KNIFE_ARC = 70f;

    // upgrades + resources
    public static float g_KNIFE_DAMAGE=4;
    public static float energyMax, energy;

    // global
    public static byte difficulty; // 0 to 4
    public static byte save;
    public static bool load=false;
    public static bool loadingFirstChunks=true;


    public void Init()
    {
        instance = this;
    }

    public static void SubtractCurrentItem()
    {
        currentItemNode.Value.count --;
        if(currentItemNode.Value.count <= 0)
        {
            string name = currentItem.name;
            RemoveCurrentItem();
            LinkedListNode<GridItem> gridItem;
            if(Inventory.instance.GetTotalCount(name, out gridItem) > 0)
            {
                Inventory.instance.Equip(gridItem);
            }
        }
        else
        {
            PlayerHUD.instance.UpdateHotbar();
        }
    }

    public static void RemoveCurrentItem()
    {
        Destroy(currentItemNode.Value.gameObject);
        currentItemNode.List.Remove(currentItemNode);
        currentItemNode = null;
        PlayerHUD.instance.UpdateHotbar();
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
