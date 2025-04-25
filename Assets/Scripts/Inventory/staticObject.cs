using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staticObject : MonoBehaviour
{
    public Building building;
    private void Start()
    {
        GameManager.instance.objects.Add(gameObject);
        building = gameObject.GetComponent<Building>();
    }
    
    
    public void Save(ref StaticObjectData data)
    {
        data.position = transform.position;
        data.buildingName = transform.name;
    }

    public void Load(StaticObjectData data)
    {
        transform.position = data.position;
    }

    private void OnDestroy()
    {
        GameManager.instance.objects.Remove(gameObject);
    }

    public void Remove()
    {
        Vector2Int position = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        BuildingGrid.instance.FreeArea(position, building.size);
        Destroy(gameObject);
    }
}
