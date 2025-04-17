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
                droppedItem.StartCollecting(transform);
            }
        }
    }
}
