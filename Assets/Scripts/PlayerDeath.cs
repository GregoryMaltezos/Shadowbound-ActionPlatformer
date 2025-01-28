using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public float deathDelay = 2f; // Delay before level restart after death.
    public Animator deathAnimator;
    public PlayerMovement playerMovement; // Reference to the player movement script.

    private bool isDead = false;

    // Call this method to handle player death.
    public void Die()
    {
        if (!isDead)
        {
            isDead = true;

            // Disable the player's movement script.
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // Play death animation if available.
            if (deathAnimator != null)
            {
                deathAnimator.SetTrigger("Death");
            }

            // Start a coroutine to restart the level after a delay.
            StartCoroutine(RestartLevelAfterDelay());
        }
    }

    // Coroutine to restart the level after a delay.
    private IEnumerator RestartLevelAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);

        // Restart the level (you should provide the correct scene name or build index).
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
