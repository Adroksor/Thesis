using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Stuff")] 
    public Vector2 playerPosition;

    public TopDownPlayerMovement playerScript;
    
    [Header("Seed")]
    public int seed;

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
    
    
    public void Save(ref PlayerPositionData data)
    {
        data.position = playerPosition;
    }

    public void Load(PlayerPositionData data)
    {
        playerScript.transform.position = data.position;
    }
}
