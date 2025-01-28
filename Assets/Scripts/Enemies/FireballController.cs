using UnityEngine;

public class FireballController : MonoBehaviour
{
    public int damage = 1; // Damage inflicted on collision with the player
    public float speed = 5.0f;
    public GameObject deathSpawnPrefab;
    public AudioClip fireballSound; // The audio clip to play when the fireball starts moving

    private AudioSource audioSource; // Reference to the AudioSource component

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Check if Rigidbody component exists before accessing it
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        else
        {
            Debug.LogWarning("Rigidbody component missing on the fireball! Adding it dynamically...");

            // Add Rigidbody component dynamically
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Now check if rb is not null before setting velocity
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("Failed to add Rigidbody component!");
        }
        // Add an AudioSource component to the GameObject if it doesn't exist
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign the audio clip to the AudioSource component
        if (fireballSound != null)
        {
            audioSource.clip = fireballSound;
            // Play the sound
            audioSource.Play();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Detected: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Destroy the fireball on collision with the player
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            // Destroy the fireball on collision with the ground or wall
            Destroy(gameObject);
        }
        GameObject deathSpawnInstance = Instantiate(deathSpawnPrefab, transform.position, Quaternion.identity);
        Destroy(deathSpawnInstance, 0.8f);
    }
}
