using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class CraftingRecipe
{
    public string[] items;
    public int[] x;
    public int[] y;
    public bool[] rotations;
    public string result_name;
    public int result_count;
}

[System.Serializable]
public class CraftingRecipiesJson
{
    public CraftingRecipe[] recipies;
}

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    // inspector
    public GameObject gridItemPrefab;
    public GameObject itemPickupPrefab;
    public Transform inventory;
    public RectTransform grid_inventory;
    public RectTransform grid_crafting;
    public RectTransform grid_result;
    public PlayerInput playerInput;
    public Rigidbody2D player_rb;
    public PlayerAnimator playerAnimator;

    [HideInInspector] public bool isOpen=false;

    [HideInInspector] public RectTransform[] grids;
    public LinkedList<GridItem> items = new LinkedList<GridItem>();
    public GridItem crafted=null;
    public CraftingRecipe[] recipies;
    public static readonly Vector2Int gridSize = new Vector2Int(9, 12);
    public const float cellSize = 32f;

    public void Init()
    {
        instance = this;
        grids = new RectTransform[2];
        grids[0] = grid_inventory;
        grids[1] = grid_crafting;
        recipies = JsonUtility.FromJson<CraftingRecipiesJson>(Resources.Load<TextAsset>("ItemData/craftingRecipies").text).recipies;
    }

    public void RemoveIngredients()
    {
        var item = items.First;
        LinkedListNode<GridItem> next;
        while(item != null)
        {
            next = item.Next;
            if(!item.Value.followMouse && item.Value.WithinGrid(grid_crafting))
            {
                item.Value.count --;
                item.Value.UpdateCount();
            }

            item = next;
        }
    }

    public void RemoveResult()
    {
        if(crafted != null)
        {
            items.Remove(crafted);
            Destroy(crafted.gameObject);
        }
    }

    public void CheckCrafting()
    {
        var craftingItems = new LinkedList<GridItem>();

        var minPos = new Vector2Int(3, 3);

        foreach(var item in items)
        {
            if(!item.followMouse && item.WithinGrid(grid_crafting))
            {
                craftingItems.AddLast(item);
                if(item.X(grid_crafting) < minPos.x) minPos.x = item.X(grid_crafting);
                if(item.Y(grid_crafting) < minPos.y) minPos.y = item.Y(grid_crafting);
            }
        }

        if(craftingItems.Count != 0)
        {
            foreach(var recipe in recipies)
            {
                if(recipe.items.Length != craftingItems.Count) continue;

                foreach(var item in craftingItems)
                {
                    for(int i=0; i<recipe.items.Length; i++)
                    {
                        if(recipe.items[i] == item.item.name && recipe.x[i] == item.X(grid_crafting)-minPos.x && recipe.y[i] == item.Y(grid_crafting)-minPos.y && (recipe.rotations.Length == 0 || item.item.size.x == item.item.size.y || recipe.rotations[i] == item.rotated))
                        {
                            goto RecipeItemSucceed;
                        }
                    }
                    break; // recipe fail

                    RecipeItemSucceed:
                    if(item == craftingItems.Last.Value)
                    {
                        RemoveResult();
                        crafted = Add(recipe.result_name, 0, 0, recipe.result_count, 0, grid_result);
                        return;
                    }
                }
            }
        }

        RemoveResult();
    }

    public GridItem Add(string item, Vector2Int pos, int count=1, int ammo=0, RectTransform grid=null)
    {
        return Add(item, pos.x, pos.y, count, ammo, grid);
    }

    public GridItem Add(string item, int x, int y, int count=1, int ammo=0, RectTransform grid=null)
    {
        if(grid == null) grid = grid_inventory;
        GameObject gameObject = Instantiate(gridItemPrefab, Vector3.zero, Quaternion.identity, inventory);
        GridItem gridItem = gameObject.GetComponent<GridItem>();
        gridItem.item = Items.items.ContainsKey(item) ? Items.items[item] : null;
        gridItem.count = count;
        gridItem.ammo = ammo;
        items.AddLast(gridItem);
        gridItem.node = items.Last;
        gridItem.Start();
        if(grid == grid_result) gridItem.crafted = true;
        gridItem.SetPos(x, y, grid);

        PlayerHUD.instance.UpdateHotbar();
        PlayerHUD.instance.UpdateAmmo();
        return gridItem;
    }

    public void Equip(LinkedListNode<GridItem> node)
    {
        PlayerState.currentItemNode = node;
        PlayerState.SwitchGun();
        PlayerHUD.instance.UpdateHotbar();
    }

    public void Clear()
    {
        LinkedListNode<GridItem> node = items.First;
        while(items.Count > 0)
        {
            if(node.Value.gameObject != null) Destroy(node.Value.gameObject);

            node = node.Next;
            items.RemoveFirst();
        }

        PlayerHUD.instance.UpdateAmmo();
    }

    public LinkedListNode<GridItem> AutoAdd(string item, int count=1, int ammo=0)
    {
        ItemStats itemStats = Items.items[item];
        if(count < itemStats.maxStack)
        {
            foreach(GridItem other in items)
            {
                if(other.item.name == item && other.count < itemStats.maxStack)
                {
                    other.count += count;
                    count = other.count - itemStats.maxStack;
                    other.count = Mathf.Min(itemStats.maxStack, other.count);
                    other.UpdateCount();
                    if(count <= 0) return other.node;
                }
            }
        }

        Add(item, 0, 0, Mathf.Min(count, itemStats.maxStack), ammo);
        GridItem gridItem = items.Last.Value;
        count -= gridItem.count;
        for(int y=gridSize.y-1; y>=0; y--)
        {
            for(int x=0; x<gridSize.x; x++)
            {
                gridItem.SetPos(x, y, grids[0]);
                for(int i=0; i<2; i++)
                {
                    if(items.Last.Value.Collides() == null && items.Last.Value.WithinGrid(grids[0]))
                    {
                        if(count == 0)
                            return items.Last;

                        return AutoAdd(item, count);
                    }
                    items.Last.Value.Rotate();
                    items.Last.Value.SetPos(x, y, grids[0]);
                }
            }
        }

        Destroy(items.Last.Value.gameObject);
        items.RemoveLast();

        return null;
    }

    public int GetTotalCount(string item)
    {
        LinkedListNode<GridItem> gridItem;
        return GetTotalCount(item, out gridItem);
    }

    public int GetTotalCount(string item, out LinkedListNode<GridItem> out_item)
    {
        out_item = null;
        int amount = 0;
        
        for(LinkedListNode<GridItem> gridItem=items.First; gridItem!=null; gridItem=gridItem.Next)
        {
            if(gridItem.Value.item.name == item)
            {
                if(amount == 0) out_item = gridItem;
                amount += gridItem.Value.count;
            }
        }

        return amount;
    }

    public void RemoveItemCount(string item, int amount)
    {
        LinkedList<GridItem> sorted = new LinkedList<GridItem>();
        LinkedListNode<GridItem> node = items.First;
        while(node != null)
        {
            if(node.Value.item.name == item)
            {
                if(sorted.Count == 0)
                {
                    sorted.AddFirst(node.Value);
                }
                else
                {
                    LinkedListNode<GridItem> sortedNode = sorted.First;
                    while(sortedNode != null)
                    {
                        if(node.Value.count < sortedNode.Value.count)
                        {
                            sorted.AddBefore(sortedNode, node.Value);
                        }
                        else if(sortedNode.Next == null)
                        {
                            sorted.AddAfter(sortedNode, node.Value);
                            break;
                        }
                        sortedNode = sortedNode.Next;
                    }
                }
            }

            node = node.Next;
        }

        node = sorted.First;
        while(node != null)
        {
            GridItem gridItem = node.Value;
            if(gridItem.count > amount)
            {
                gridItem.count -= amount;
                gridItem.UpdateCount();
                PlayerHUD.instance.UpdateAmmo();
                return;
            }
            else
            {
                amount -= gridItem.count;
                Destroy(gridItem.gameObject);
                items.Remove(node.Value);
            }
            node = node.Next;

        }

        PlayerHUD.instance.UpdateAmmo();
    }

    public void Open()
    {
        isOpen = true;
        foreach(GridItem item in items)
        {
            item.highlighted = false;
        }
    }

    public void Close()
    {
        RemoveResult();

        LinkedListNode<GridItem> node = items.First;
        LinkedListNode<GridItem> next;
        
        while(node != null)
        {
            next = node.Next;
            if(!node.Value.WithinGrid(grids[0]) || node.Value.followMouse)
            {
                // DROP
                
                if(PlayerState.currentItemNode == node)
                {
                    PlayerState.currentItemNode = null;
                }

                GameObject item = Instantiate(itemPickupPrefab, player_rb.position, Quaternion.identity, Entities.t);
                ItemPickup pickup = item.GetComponent<ItemPickup>();
                pickup.Init(node.Value.item.name, node.Value.count, node.Value.ammo, true);
                Destroy(node.Value.gameObject);
                items.Remove(node);
            }
            node = next;
        }

        PlayerHUD.instance.UpdateHotbar();
        PlayerHUD.instance.UpdateAmmo();
        
        isOpen = false;
    }
}
