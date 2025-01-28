using UnityEngine;

public class SpawnPrefabOnTrigger : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnPoint;

    private GameObject instantiatedPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the object that entered the trigger is the player
        {
            if (instantiatedPrefab != null)
            {
                Destroy(instantiatedPrefab); // Destroy the previously instantiated prefab
            }

            instantiatedPrefab = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
    }

    // Add a method to destroy the prefab explicitly
    public void DestroyPrefab()
    {
        if (instantiatedPrefab != null)
        {
            Destroy(instantiatedPrefab);
        }
    }
}
