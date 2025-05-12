using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour
{
    public Building building;
    private void Start()
    {
        building = gameObject.GetComponent<Building>();
        
        if (!building.isGhost)
        {
            GameManager.instance.objects.Add(gameObject);
        }
    }
    
    
    public void Save(ref StaticObjectData data)
    {
        data.position = transform.position;
        data.buildingName = transform.name;
    }

    public void Load(StaticObjectData data)
    {
        transform.position = data.position;
        building.isGhost = false;
    }

    private void OnDestroy()
    {
        GameManager.instance.objects.Remove(gameObject);
    }
}
