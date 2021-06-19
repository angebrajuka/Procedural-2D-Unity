using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GridItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    bool highlighted;

    public void OnPointerEnter(PointerEventData eventData) {
        highlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        highlighted = false;
    }
    
    public Item item;
    RectTransform rectTransform;
    public bool followMouse=false;
    public LinkedListNode<GridItem> node;
    bool rotated=false;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(GetWidth(), GetHeight(), 1);
        GetComponent<Image>().sprite = Items.items[(int)item].sprite;
    }

    int GetX() { return (int)Mathf.Floor(rectTransform.localPosition.x/Inventory.cellSize); }
    int GetY() { return (int)Mathf.Floor(rectTransform.localPosition.y/Inventory.cellSize); }
    int GetWidth(bool recurse=false) { return rotated&&!recurse ? GetHeight(true) : Items.items[(int)item].size.x; }
    int GetHeight(bool recurse=false) { return rotated&&!recurse ? GetWidth(true) : Items.items[(int)item].size.y; }

    public bool WithinGrid() {
        return (GetX() >= 0 && GetY() >= 0 && GetX()+GetWidth() <= Inventory.gridSize.x && GetY()+GetHeight() <= Inventory.gridSize.y);
    }

    public void SetPos(int x, int y) {
        transform.localPosition = Vector3.right*Inventory.cellSize*(x) + Vector3.up*Inventory.cellSize*(y);
    }

    public void OnClick() {
        if(followMouse) {
            foreach(GridItem item in Inventory.items) {
                if(item == this) continue;
                if(GetX()+GetWidth() > item.GetX() && GetX() < item.GetX()+item.GetWidth() && GetY()+GetHeight() > item.GetY() && GetY() < item.GetY()+item.GetHeight())
                    return;
            }
            if(WithinGrid()) {
                SetPos(GetX(), GetY());
                rectTransform.pivot = rotated ? Vector2.right : Vector2.zero;
            }
        } else {
            transform.SetAsLastSibling();
            rectTransform.pivot = Vector2.right*0.5f/GetWidth(true)+Vector2.up*0.5f/GetHeight(true);
            if(rotated) rectTransform.pivot = new Vector2(1-rectTransform.pivot.x, rectTransform.pivot.y);//.x*Vector2.right + rectTransform.pivot.y*Vector2.up;
        }

        followMouse=!followMouse;
        // rectTransform.pivot = Vector2.right*0.5f/GetWidth()+Vector2.up*0.5f/GetHeight()-rectTransform.pivot;
    }

    void Update() {
        if(followMouse) {
            rectTransform.position = Input.mousePosition;
            if(Input.GetKeyDown(PlayerInput.keybinds[PlayerInput.key_i_rotate])) {
                rotated = !rotated;
                rectTransform.eulerAngles = Vector3.back*90-rectTransform.eulerAngles;
                rectTransform.pivot = new Vector2(1-rectTransform.pivot.x, rectTransform.pivot.y);//= rotated ? :Vector2.right - (rectTransform.pivot*Vector2.right);
            }
        } else if(highlighted) {
            if(Input.GetKeyDown(PlayerInput.keybinds[PlayerInput.key_i_equip])) {
                if(Items.items[(int)item].gun == -1) {
                    PlayerStats.currentItem = item;
                    PlayerStats.currentItemNode = node;
                    PlayerStats.hud.UpdateHotbar();
                } else {
                    PlayerStats.SwitchGun(Items.items[(int)item].gun);
                    PlayerStats.currentGunItemNode = node;
                }
            }
        }
    }
}
