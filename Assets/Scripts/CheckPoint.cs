using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Sprite normalSprite;  // The initial sprite
    public Sprite changedSprite; // The sprite to change to
    public Transform respawnPoint; // Respawn point

    private bool playerPassed = false;
    public RespawnManager respawnManager; // Assign the RespawnManager in the Inspector

    public AudioSource checkpointSound; // Reference to the AudioSource

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerPassed)
        {
            // Play the checkpoint sound
            if (checkpointSound != null)
            {
                checkpointSound.Play();
            }

            // Change the sprite when the player passes
            GetComponent<SpriteRenderer>().sprite = changedSprite;
            playerPassed = true;

            // Set a respawn point using the assigned RespawnManager
            if (respawnManager != null && respawnPoint != null)
            {
                respawnManager.SetRespawnPoint(respawnPoint.position);
            }
        }
    }
}
