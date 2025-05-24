using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Stuff")]
    public GameObject player;
    public Vector2 playerPosition;

    public TopDownPlayerMovement PlayerMovement;
    
    public BuildingPlacer buildingPlacer;
    
    [Header("Lists of objects")]
    public List<GameObject> objects = new List<GameObject>();
    public List<GameObject> chests = new List<GameObject>();
    public List<GameObject> furnaces = new List<GameObject>();
    public List<GameObject> workbenches = new List<GameObject>();
    public List<GameObject> entitiesPigs = new List<GameObject>();
    public List<GameObject> resources = new List<GameObject>();
    public List<GameObject> droppedItems = new List<GameObject>();

    public GameObject[] buildingPrefabs;
    public GameObject[] entityPrefabs;

    public GameObject exitMenu;
    public CinemachineVirtualCamera cineCamera;
    public PixelPerfectCamera ppc;

    [Header("DebugText")] public TextMeshProUGUI cameraZoomText;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        cameraZoomText.text = ppc.assetsPPU.ToString();
    }


    public GameObject GetObjectByName(string name)
    {
        return buildingPrefabs.FirstOrDefault(prefab => prefab.name == name);
    }

    public GameObject GetEntitytByName(string name)
    {
        return entityPrefabs.FirstOrDefault(prefab => prefab.name == name);
    }

}
