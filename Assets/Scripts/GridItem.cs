using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GridItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
    
    public ItemStats item;
    public int count=1;
    public int ammo=0;
    public bool crafted; // not craftable items, just the item that was crafted last, to clear crafting table when removing
    RectTransform rectTransform;
    public bool followMouse;
    public LinkedListNode<GridItem> node;
    RectTransform image;
    EventTrigger eventTrigger;
    public bool rotated;

    public void Start()
    {
        if(rectTransform != null) return;
        eventTrigger = GetComponent<EventTrigger>();
        rotated = false;
        highlighted = false;
        followMouse = false;
        crafted = false;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(item.size.x*Inventory.cellSize, item.size.y*Inventory.cellSize);
        Transform child = transform.GetChild(0);
        image = child.GetComponent<RectTransform>();
        image.sizeDelta = rectTransform.sizeDelta;
        child.GetComponent<Image>().sprite = item.sprite;
        UpdateCount();
    }

    public void UpdateCount()
    {
        Text countText = transform.GetChild(0).GetChild(4).GetComponent<Text>();
        countText.text = count == 1 ? "" : ""+count;
        if(count == 0)
        {
            Inventory.instance.items.Remove(node);
            Destroy(gameObject);
        }
    }

    float rawX                  { get { return rectTransform.offsetMin.x; } }
    float rawY                  { get { return rectTransform.offsetMin.y; } }
    float rawW                  { get { return W*Inventory.cellSize; } }
    float rawH                  { get { return H*Inventory.cellSize; } }
    public int X(RectTransform grid)  { return (int)Mathf.Round((rawX-GridXRaw(grid))/Inventory.cellSize); }
    public int Y(RectTransform grid)  { return (int)Mathf.Round((rawY-GridYRaw(grid))/Inventory.cellSize); }
    int W                       { get { return rotated ? item.size.y : item.size.x; } }
    int H                       { get { return rotated ? item.size.x : item.size.y; } }

    float GridXRaw(RectTransform grid) { return grid.offsetMin.x; }
    float GridYRaw(RectTransform grid) { return grid.offsetMin.y; }
    float GridWRaw(RectTransform grid) { return grid.sizeDelta.x; }
    float GridHRaw(RectTransform grid) { return grid.sizeDelta.y; }
    float GridX(RectTransform grid)    { return GridXRaw(grid)/Inventory.cellSize; }
    float GridY(RectTransform grid)    { return GridYRaw(grid)/Inventory.cellSize; }
    float GridW(RectTransform grid)    { return GridWRaw(grid)/Inventory.cellSize; }
    float GridH(RectTransform grid)    { return GridHRaw(grid)/Inventory.cellSize; }

    public bool WithinGrid(RectTransform grid)
    {
        return (X(grid) >= 0 && Y(grid) >= 0 && X(grid)+W <= GridW(grid) && Y(grid)+H <= GridH(grid));
    }

    public bool WithinGridRaw(RectTransform grid)
    {
        return (rawX+rawW > GridXRaw(grid) && rawX < GridXRaw(grid)+GridWRaw(grid) && rawY+rawH > GridYRaw(grid) && rawY < GridYRaw(grid)+GridHRaw(grid));
    }

    public void SetPos(int x, int y, RectTransform grid)
    {
        rectTransform.offsetMin = new Vector2(GridXRaw(grid)+x*Inventory.cellSize, GridYRaw(grid)+y*Inventory.cellSize);
        rectTransform.offsetMax = rectTransform.offsetMin+new Vector2(rawW, rawH);
    }

    public void SnapToGrid(RectTransform grid)
    {
        var pos = SnappedToGrid(grid);
        SetPos(pos.x, pos.y, grid);
    }

    public Vector2Int SnappedToGrid(RectTransform grid)
    {
        return new Vector2Int(X(grid), Y(grid));
    }

    public LinkedListNode<GridItem> Collides()
    {
        LinkedListNode<GridItem> other = Inventory.instance.items.First;
        while(other != null)
        {
            if(other.Value != this && rawX+rawW > other.Value.rawX && rawX < other.Value.rawX+other.Value.rawW && rawY+rawH > other.Value.rawY && rawY < other.Value.rawY+other.Value.rawH)
                return other;
            
            other = other.Next;
        }

        return null;
    }

    public LinkedListNode<GridItem> CollidesSnap(RectTransform grid)
    {
        LinkedListNode<GridItem> other = Inventory.instance.items.First;
        while(other != null)
        {
            if(other.Value != this && X(grid)+W > other.Value.X(grid) && X(grid) < other.Value.X(grid)+other.Value.W && Y(grid)+H > other.Value.Y(grid) && Y(grid) < other.Value.Y(grid)+other.Value.H)
                return other;
            
            other = other.Next;
        }

        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Middle) return;

        if(followMouse)
        {
            if(WithinGridRaw(Inventory.instance.grid_result)) return;
            foreach(var grid in Inventory.instance.grids)
            {
                var collides = CollidesSnap(grid);
                if(WithinGrid(grid) && (collides == null || collides.Value.item == item))
                {
                    int _count = eventData.button == PointerEventData.InputButton.Left ? count : 1;

                    if(collides == null)
                    {
                        GridItem gridItem = Inventory.instance.Add(item.name, SnappedToGrid(grid), _count, 0, grid);
                        if(rotated) gridItem.Rotate();
                        count -= _count;
                        UpdateCount();
                        goto CheckCrafting;
                    }
                    else
                    {
                        int diff = Mathf.Min(collides.Value.item.maxStack - collides.Value.count, count, _count);
                        collides.Value.count += diff;
                        collides.Value.UpdateCount();
                        count -= diff;
                        UpdateCount();
                        goto CheckCrafting;
                    }
                }
            }
            foreach(var grid in Inventory.instance.grids)
            {
                if(Collides() != null || (WithinGridRaw(grid) && !WithinGrid(grid)))
                {
                    return;
                }
            }
        }
        else
        {
            if(crafted)
            {
                crafted = false;
                Inventory.instance.crafted = null;
                Inventory.instance.RemoveIngredients();
            }
        }

        // change pivot without moving:
        Vector2 pivot = Vector2.one*0.5f-rectTransform.pivot;
        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;

        followMouse=!followMouse;
        eventTrigger.enabled = !followMouse;

        CheckCrafting:
        Inventory.instance.CheckCrafting();
    }

    public void Rotate()
    {
        rotated = !rotated;
        image.eulerAngles = Vector3.back*90-image.eulerAngles;
        Vector2 size = rectTransform.sizeDelta;
        rectTransform.sizeDelta = new Vector2(size.y, size.x);
    }

    void Update()
    {
        if(followMouse)
        {
            transform.SetAsLastSibling();
            rectTransform.position = Input.mousePosition;
            if(Input.GetKeyDown(PlayerInput.keybinds[Keybind.i_rotate]))
            {
                Rotate();
            }
        }
        else if(highlighted)
        {
            if(item.equipable && Input.GetKeyDown(PlayerInput.keybinds[Keybind.i_equip]))
            {
                if(PlayerState.currentItem == item)
                {
                    PlayerState.currentItemNode = null;
                }
                else
                {
                    Inventory.instance.Equip(node);
                }
            }
        }
    }
}
