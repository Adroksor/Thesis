using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PickUp"))
        {
            DroppedItem droppedItem = other.GetComponent<DroppedItem>();
            if (droppedItem != null)
            {
                int remaining = InventoryManager.instance.TryAddItemToInventoryData(
                    ItemDatabaseInstance.Instance.GetItemByName(droppedItem.itemName),
                    droppedItem.amount,
                    InventoryManager.instance.playerInventoryScript.inventoryData
                );

                if (remaining <= 0)
                {
                    Destroy(other.gameObject); // all picked up
                }
                else
                {
                    droppedItem.amount = remaining; // update what's left
                }
            }
        }
    }
}
