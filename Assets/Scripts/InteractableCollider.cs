using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCollider : MonoBehaviour
{
    public Building building;
    public BoxCollider2D boxCollider;

    private void Start()
    {
        building = gameObject.GetComponentInParent<Building>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector3(building.size.x, building.size.y, 1);
    }
}
