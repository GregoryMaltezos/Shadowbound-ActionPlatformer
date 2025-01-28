using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private bool isAttacking = false;
    private float idleDelay = 1.0f; // Adjust the delay time as needed
    private PlayerStatus playerStatus;
    private Rigidbody2D rb;

    // Add a separate audio source for the attack sound
    public AudioSource attackAudioSource;
    public AudioClip attackSound;

    private void Start()
    {
        // Get the Animator component on the player character
        animator = GetComponent<Animator>();

        // Get the PlayerStatus component on the player character
        playerStatus = GetComponent<PlayerStatus>();

        // Get the Rigidbody2D component
        rb = GetComponent < Rigidbody2D>();

        // Initialize the attack audio source
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.clip = attackSound;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && playerStatus.hasWeapon) // Left mouse button and player has the weapon
        {
            // Trigger the "Attack" animation
            animator.SetTrigger("Attack");
            isAttacking = true;

            // Play the attack sound
            attackAudioSource.Play();

            // Start a coroutine to trigger "Idle" after a delay
            StartCoroutine(IdleAfterDelay());
        }
    }

        private IEnumerator IdleAfterDelay()
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(0.5f);

            // Reset the "Idle" trigger
            animator.ResetTrigger("Idle");

            // Trigger the "Idle" animation to transition back to the idle state
            animator.SetTrigger("Idle");
            isAttacking = false;
        }

    



    public void PlayerReflect(GameObject fireball)
    {
        if (isAttacking)
        {
            ReflectableFireball reflectableFireball = fireball.GetComponent<ReflectableFireball>();

            if (reflectableFireball != null && reflectableFireball.IsReflectable())
            {
                // Reflect the fireball by reversing its direction
                Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
                fireballRb.velocity = -fireballRb.velocity;

                // Change the rotation of the fireball by -180 degrees
                fireball.transform.Rotate(0f, 0f, -180f);

                // Set the "reflected" flag on the fireball to prevent further reflection
                reflectableFireball.SetReflectable(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered!");

        if (isAttacking)
        {
            // Check if the collided object is an attackable enemy
            if (other.CompareTag("Enemy") || other.GetComponent<IAttackable>() != null)
            {
                Debug.Log("Collision with Enemy!");
                IAttackable enemy = other.GetComponent<IAttackable>();
                if (enemy != null)
                {
                    enemy.TakeDamage(playerStatus.knifeDamage);
                }
            }

            // Check if the collided object is a reflectable fireball
            if (other.CompareTag("ReflectableFireball"))
            {
                // Call the PlayerReflect method to reflect the fireball
                PlayerReflect(other.gameObject);
            }
        }
    }
}
