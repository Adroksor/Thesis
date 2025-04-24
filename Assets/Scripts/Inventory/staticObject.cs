using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staticObject : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.objects.Add(gameObject);
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
        Destroy(gameObject);
    }
}
