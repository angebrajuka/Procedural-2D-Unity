using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GridItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    public bool highlighted;

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlighted = false;
    }
    
    public Item item;
    public int count=1;
    public int ammo=0;
    RectTransform rectTransform;
    RectTransform grid;
    public bool followMouse;
    public LinkedListNode<GridItem> node;
    RectTransform image;
    EventTrigger eventTrigger;
    public bool rotated;

    public void Start()
    {
        grid = transform.parent.GetComponent<RectTransform>();
        if(rectTransform != null) return;
        eventTrigger = GetComponent<EventTrigger>();
        rotated = false;
        highlighted = false;
        followMouse = false;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Items.items[(int)item].size.x*Inventory.cellSize, Items.items[(int)item].size.y*Inventory.cellSize);
        Transform child = transform.GetChild(0);
        image = child.GetComponent<RectTransform>();
        image.sizeDelta = rectTransform.sizeDelta;
        child.GetComponent<Image>().sprite = Items.items[(int)item].sprite;
        UpdateCount();
    }

    public void UpdateCount()
    {
        Text countText = transform.GetChild(0).GetChild(4).GetComponent<Text>();
        if(count == 1)  countText.text = "";
        else            countText.text = ""+count;
    }

    public int GetX()       { return (int)Mathf.Round((rectTransform.localPosition.x-rectTransform.sizeDelta.x/2)/Inventory.cellSize); }
    public int GetY()       { return (int)Mathf.Round((rectTransform.localPosition.y-rectTransform.sizeDelta.y/2)/Inventory.cellSize); }
    int GetWidth()          { return (int)Mathf.Round(rectTransform.sizeDelta.x/Inventory.cellSize); }
    int GetHeight()         { return (int)Mathf.Round(rectTransform.sizeDelta.y/Inventory.cellSize); }

    float RawX()        { return rectTransform.localPosition.x; }
    float RawY()        { return rectTransform.localPosition.y; }
    float RawWidth()    { return rectTransform.sizeDelta.x; }
    float RawHeight()   { return rectTransform.sizeDelta.y; }


    public bool WithinGrid()
    {
        return (GetX() >= 0 && GetY() >= 0 && GetX()+GetWidth() <= Inventory.gridSize.x && GetY()+GetHeight() <= Inventory.gridSize.y);
    }

    public bool WithinGridRaw()
    {
        return (RawX()+RawWidth()/2 > 0 && RawX()-RawWidth()/2 < grid.sizeDelta.x && RawY()+RawHeight()/2 > 0 && RawY()-RawHeight()/2 < grid.sizeDelta.y);
    }

    public void SetPos(int x, int y)
    {
        rectTransform.localPosition = Vector3.right*Inventory.cellSize*(x+GetWidth()/2f) + Vector3.up*Inventory.cellSize*(y+GetHeight()/2f);
    }

    public LinkedListNode<GridItem> Collides()
    {
        LinkedListNode<GridItem> other = PlayerStats.inventory.items.First;
        while(other != null)
        {
            if(other.Value != this && GetX()+GetWidth() > other.Value.GetX() && GetX() < other.Value.GetX()+other.Value.GetWidth() && GetY()+GetHeight() > other.Value.GetY() && GetY() < other.Value.GetY()+other.Value.GetHeight())
                return other;
            
            other = other.Next;
        }

        return null;
    }

    public void OnClick()
    {
        if(followMouse)
        {
            if(Collides() != null || (WithinGridRaw() && !WithinGrid())) return;
            if(WithinGrid()) SetPos(GetX(), GetY());
        }
        else
        {
            transform.SetAsLastSibling();
        }

        followMouse=!followMouse;
        eventTrigger.enabled = !followMouse;
    }

    public void Rotate()
    {
        int x=GetX();
        int y=GetY();
        rotated = !rotated;
        image.eulerAngles = Vector3.back*90-image.eulerAngles;
        Vector2 size = rectTransform.sizeDelta;
        rectTransform.sizeDelta = new Vector2(size.y, size.x);
        if(!followMouse) SetPos(x, y);
    }

    void Update()
    {
        if(followMouse)
        {
            rectTransform.position = Input.mousePosition;
            if(Input.GetKeyDown(PlayerInput.keybinds[Keybind.i_rotate]))
            {
                Rotate();
            }
        }
        else if(highlighted)
        {
            if(Items.items[(int)item].equipable && Input.GetKeyDown(PlayerInput.keybinds[Keybind.i_equip]))
            {
                if(PlayerStats.currentItemNode == node)
                {
                    PlayerStats.currentItem = Item.NONE;
                    PlayerStats.SwitchGun(-1, true);
                }
                else
                {
                    PlayerStats.currentItemNode = node;
                    if(Items.items[(int)item].gun == -1)
                    {
                        PlayerStats.currentItem = item;
                        PlayerStats.SwitchGun(-1, false);
                        PlayerStats.hud.UpdateHotbar();
                    }
                    else
                    {
                        PlayerStats.SwitchGun(Items.items[(int)item].gun, false);
                    }
                }
            }
        }
    }
}
