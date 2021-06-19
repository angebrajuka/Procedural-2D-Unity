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

    public static LinkedList<GridItem> items = new LinkedList<GridItem>();
    public static readonly Vector2Int gridSize = new Vector2Int(9, 12);
    public const float cellSize = 384f/9f;

    void Add(Item item, int x, int y) {
        GameObject gameObject = Instantiate(gridItemPrefab, grid);
        GridItem gridItem = gameObject.GetComponent<GridItem>();
        gridItem.item = item;
        items.AddLast(gridItem);
        gridItem.node = items.Last;
        gridItem.SetPos(x, y);
    }

    void Start() {
        Add(Item.FLASHLIGHT, 0, 0);
        Add(Item.BOMB, 0, 1);
        Add(Item.FISHING_ROD, 0, 3);
        Add(Item.ENERGY_RIFLE, 3, 0);
    }

    public void Open() {
        
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
    }

    void Update() {    
        // highlight.localPosition = Vector3.down*(PlayerStats._nextGun-1)*120+Vector3.left*160;

        if(Input.GetKeyDown(PlayerInput.keybinds[PlayerInput.key_inventory])) {
            Close();
            PauseHandler.UnPause();
            PauseHandler.UnBlur();
            playerInput.enabled = true;
            PlayerStats.hud.transform.gameObject.SetActive(true);
            transform.gameObject.SetActive(false);
        }
    }
}
