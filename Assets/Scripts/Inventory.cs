using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public GameObject gridItemPrefab;
    public GameObject itemPickupPrefab;
    public Transform grid;
    public PlayerInput playerInput;
    public Rigidbody2D player_rb;
    public PlayerAnimator playerAnimator;

    public static bool isOpen=false;

    public static LinkedList<GridItem> items = new LinkedList<GridItem>();
    public static readonly Vector2Int gridSize = new Vector2Int(9, 12);
    public const float cellSize = 384f/9f;

    void Add(Item item, int x, int y) {
        GameObject gameObject = Instantiate(gridItemPrefab, Vector3.zero, Quaternion.identity, grid);
        GridItem gridItem = gameObject.GetComponent<GridItem>();
        gridItem.item = item;
        items.AddLast(gridItem);
        gridItem.node = items.Last;
        gridItem.Start();
        gridItem.SetPos(x, y);
    }

    public bool AutoAdd(Item item) {
        
        Add(item, 0, 0);
        
        for(int y=gridSize.y-1; y>=0; y--) {
            for(int x=0; x<gridSize.x; x++) {
                items.Last.Value.SetPos(x, y);
                for(int i=0; i<2; i++) {
                    if(!items.Last.Value.Collides() && items.Last.Value.WithinGrid()) {
                        return true;
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

    public void Open() {
        isOpen = true;
        foreach(GridItem item in items) {
            item.highlighted = false;
        }
    }

    public void Close() {
        LinkedListNode<GridItem> node = items.First;
        LinkedListNode<GridItem> next;
        while(node != null) {
            next = node.Next;
            if(!node.Value.WithinGrid() || node.Value.followMouse) {
                
                // DROP

                if(PlayerStats.currentItemNode == node) {
                    PlayerStats.currentItem = Item.NONE;
                    PlayerStats.currentItemNode = null;
                    PlayerStats.hud.UpdateHotbar();
                } else if(PlayerStats.currentGunItemNode == node) {
                    PlayerStats.currentGun = null;
                    PlayerStats.currentGunItemNode = null;
                    PlayerStats.hud.UpdateHotbar();
                    PlayerStats.hud.UpdateAmmo();
                }


                GameObject item = Instantiate(itemPickupPrefab, player_rb.position, Quaternion.identity);
                item.GetComponent<ItemPickup>().item = node.Value.item;
                SpriteRenderer sprite = item.GetComponent<SpriteRenderer>();
                sprite.sprite = Items.items[(int)node.Value.item].sprite;
                item.GetComponent<Rigidbody2D>().AddForce(new Vector2((Random.value-0.5f)*100, (Random.value-0.5f)*100));
                Destroy(node.Value.gameObject);
                node.List.Remove(node);
            }
            node = next;
        }

        playerAnimator.UpdateGunImage();
        isOpen = false;
    }

    
}
