using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public ItemData itemData;
    public int itemCount;
    public ItemUI itemUI;
    public InventoryUI parentPage;

    public Action<InventorySlotUI, InventoryUI> onBeginDragLeft, onBeginDragRight, onEndDrag, onDropRight, onDropLeft, onClick;

    private void Start()
    {
        parentPage = transform.parent.GetComponent<InventoryUI>();

        SetData(itemData, itemCount);
    }

    public void SetData(ItemData itemData, int itemCount)
    {
        if (itemData == null || itemCount <= 0)
        {
            itemUI.gameObject.SetActive(false);
            this.itemData = null;
            this.itemCount = 0;
        }
        else
        {
            itemUI.gameObject.SetActive(true);
            this.itemData = itemData;
            this.itemCount = itemCount;
            itemUI.itemIcon = itemData.ItemImage;
            itemUI.itemCount = itemCount;
            itemUI.UpdateItemUI();
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onBeginDragLeft?.Invoke(this, parentPage);
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            onBeginDragRight?.Invoke(this, parentPage);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(this, parentPage);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            onDropRight?.Invoke(this, parentPage);
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onDropLeft?.Invoke(this, parentPage);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this, parentPage);
    }
}
