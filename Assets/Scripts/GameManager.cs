using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Stuff")] 
    public Vector2 playerPosition;

    public TopDownPlayerMovement playerScript;
    
    public BuildingPlacer buildingPlacer;
    
    [Header("Seed")]
    public int seed;
    [Header("Lists of objects")]
    public List<GameObject> objects = new List<GameObject>();
    public List<GameObject> chests = new List<GameObject>();
    
    public GameObject[] buildingPrefabs;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        seed = Random.Range(100_000, 999_999);
    }
    
    
    public GameObject GetObjectByName(string name)
    {
        return buildingPrefabs.FirstOrDefault(prefab => prefab.name == name);
    }
    
    
    public void Save(ref PlayerPositionData data)
    {
        data.position = playerPosition;
    }

    public void Load(PlayerPositionData data)
    {
        playerScript.transform.position = data.position;
    }
    
    
}
