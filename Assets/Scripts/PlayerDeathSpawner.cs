using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathSpawner : MonoBehaviour
{
    public GameObject deathPrefab; // Reference to the prefab to spawn on player's death
    public float spawnHeight = 3.0f; // Height above the player to spawn the deathPrefab

    // Subscribe to the player's death event
    void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += SpawnDeathPrefab;
    }

    // Unsubscribe from the player's death event
    void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= SpawnDeathPrefab;
    }

    // Spawn the deathPrefab above the stored death location
    private void SpawnDeathPrefab(Vector3 location)
    {
        if (deathPrefab != null)
        {
            Vector3 spawnPosition = location + Vector3.up * spawnHeight;
            Instantiate(deathPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
