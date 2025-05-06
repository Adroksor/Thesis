using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PigSpawner : MonoBehaviour
{
    [Tooltip("Pig prefab that has AnimalWander + WalkTween set up")]
    public GameObject pigPrefab;

    [Tooltip("How far from the player the pigs appear")]
    public float radius = 4f;

    [Tooltip("Amount to random‑jitter each spawn so the circle looks natural")]
    public float positionNoise = 0.4f;

    private void Start()
    {
        //SpawnTenPigs();
    }

    [ContextMenu("Spawn 10 Pigs Around Player")]
    public void SpawnTenPigs()
    {
        if (pigPrefab == null)
        {
            Debug.LogError("Pig prefab reference missing!");
            return;
        }

        Vector3 playerPosition = GameManager.instance.playerPosition;

        const int pigCount = 10;
        float angleStep = 360f / pigCount;

        for (int i = 0; i < pigCount; i++)
        {
            // Evenly spaced angle around the circle
            float angleDeg = i * angleStep;
            float rad      = angleDeg * Mathf.Deg2Rad;

            // Base position on perfect circle
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;

            // Small noise so they’re not on a perfect ring
            offset += (Vector3)Random.insideUnitCircle * positionNoise;

            Vector3 spawnPos = playerPosition + offset;

            // Instantiate
            Instantiate(pigPrefab, spawnPos, Quaternion.identity);
        }
    }
}