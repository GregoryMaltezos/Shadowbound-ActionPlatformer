using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public int damageAmount = 1; // Amount of damage to inflict on the player.
    public Sprite triggeredSprite; // The sprite to use when the trap is triggered.

    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;
    private bool isTriggered = false;
    public Vector2 knockbackDirection = new Vector2(1.0f, 1.0f);
  
    private void Start()
    {
        // Get the SpriteRenderer component.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save the original sprite.
        originalSprite = spriteRenderer.sprite;

       

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player and the trap is not already triggered.
        if (!isTriggered && other.CompareTag("Player"))
        {
            // Get the PlayerHealth script on the player.
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            KnockBack knockback = other.GetComponent<KnockBack>();
            if (knockback != null)
            {
                knockback.TakeDamage(knockbackDirection);
            }


            if (playerHealth != null)
            {
                // Inflict damage on the player.
                playerHealth.TakeDamage(damageAmount);

                // Change the sprite to the triggered sprite.
                spriteRenderer.sprite = triggeredSprite;
                isTriggered = true;

                // Start a coroutine to reset the sprite after a few seconds.
                StartCoroutine(ResetSprite());
            }
        }

    }
        private IEnumerator ResetSprite()
        {
            // Wait for a few seconds.
            yield return new WaitForSeconds(3f); // Change the time duration as needed.

            // Restore the original sprite.
            spriteRenderer.sprite = originalSprite;
            isTriggered = false;
        }
    
}
