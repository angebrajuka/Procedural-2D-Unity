using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    // hierarchy
    public GameObject gridItemPrefab;
    public GameObject itemPickupPrefab;
    public Transform grid;
    public PlayerInput playerInput;
    public Rigidbody2D player_rb;
    public PlayerAnimator playerAnimator;
    public Transform entities;

    [HideInInspector] public bool isOpen=false;

    public LinkedList<GridItem> items = new LinkedList<GridItem>();
    public static readonly Vector2Int gridSize = new Vector2Int(9, 12);
    public const float cellSize = 384f/9f;

    public void Init()
    {
        instance = this;
    }

    public GridItem Add(string item, int x, int y, int count=1, int ammo=0)
    {
        GameObject gameObject = Instantiate(gridItemPrefab, Vector3.zero, Quaternion.identity, grid);
        GridItem gridItem = gameObject.GetComponent<GridItem>();
        gridItem.item = Items.items.ContainsKey(item) ? Items.items[item] : null;
        gridItem.count = count;
        gridItem.ammo = ammo;
        items.AddLast(gridItem);
        gridItem.node = items.Last;
        gridItem.Start();
        gridItem.SetPos(x, y);

        PlayerHUD.instance.UpdateHotbar();
        PlayerHUD.instance.UpdateAmmo();
        return gridItem;
    }

    public void Equip(LinkedListNode<GridItem> node)
    {
        PlayerState.currentItemNode = node;
        PlayerState.SwitchGun(false);
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

    public bool AutoAdd(string item, int count=1, int ammo=0)
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
                    if(count <= 0) return true;
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
                gridItem.SetPos(x, y);
                for(int i=0; i<2; i++)
                {
                    if(items.Last.Value.Collides() == null && items.Last.Value.WithinGrid())
                    {
                        if(count == 0)
                            return true;

                        return AutoAdd(item, count);
                    }
                    items.Last.Value.Rotate();
                    items.Last.Value.SetPos(x, y);
                }
            }
        }

        Destroy(items.Last.Value.gameObject);
        items.RemoveLast();

        return false;
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
        LinkedListNode<GridItem> node = items.First;
        LinkedListNode<GridItem> next;
        
        while(node != null)
        {
            next = node.Next;
            if(!node.Value.WithinGrid() || node.Value.followMouse)
            {
                // DROP
                
                if(PlayerState.currentItemNode == node)
                {
                    PlayerState.SwitchGun(true);
                }

                GameObject item = Instantiate(itemPickupPrefab, player_rb.position, Quaternion.identity, Entities.t);
                item.transform.parent = entities;
                ItemPickup pickup = item.GetComponent<ItemPickup>();
                pickup.item = node.Value.item.name;
                pickup.count = node.Value.count;
                pickup.ammo = node.Value.ammo;
                SpriteRenderer sprite = item.GetComponent<SpriteRenderer>();
                sprite.sprite = node.Value.item.sprite;
                item.GetComponent<Rigidbody2D>().AddForce(new Vector2((Random.value-0.5f)*100, (Random.value-0.5f)*100));
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
