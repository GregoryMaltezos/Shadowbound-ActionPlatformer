using System.Collections;
using UnityEngine;

public class CharacterSpaw : MonoBehaviour
{
    public GameObject playerPrefab; // Your player character prefab
    public Transform spawnPoint;   // The spawn point for your player

    private Animator animator;
    private static bool hasSpawned = false; // Flag to ensure the animation is triggered only once

    private void Start()
    {
        if (hasSpawned)
        {
            // If a player has already spawned, disable this script on this GameObject.
            enabled = false;
            return;
        }

        // Otherwise, this is the first instance, so proceed with the spawn animation.
        hasSpawned = true;

        animator = GetComponent<Animator>(); // Assuming the spawn animation is part of an object with an Animator component

        // Trigger the spawn animation when the game starts
        StartCoroutine(SpawnPlayerWithDelay());
    }

    private IEnumerator SpawnPlayerWithDelay()
    {
        // Optionally, you can enable or disable other components or scripts during spawn
        // For example, disable the player's movement script during spawn animation
        PlayerMovement playerMovement = playerPrefab.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Trigger the spawn animation (assuming you have a "Spawn" trigger in your Animator)
        animator.SetTrigger("Spawn");

        // You can set a delay for the entire spawn process, including the animation
        float spawnDelay = 0.3f; // Adjust this value according to your animation length

        yield return new WaitForSeconds(spawnDelay);

        // Instantiate the player prefab at the spawn point
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        // Enable the player's control script
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}
