using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour, IAttackable
{
    public int maxHP = 2; // Maximum HP of the enemy
    private int currentHP;

    public Animator animator; // Reference to the Animator component
    public string damageAnimationState = "Damage"; // Name of the damage animation state
    public string destroyAnimationState = "Destroy"; // Name of the destroy animation state

    public int damageAmount = 1; // Amount of damage to inflict on the player.
    private bool isTriggered = false;
    public Vector2 knockbackDirection = new Vector2(1.0f, 1.0f);

    public Transform leftPatrolPoint;
    public Transform rightPatrolPoint;
    public float moveSpeed = 2f;
    public float detectionRadius = 5f;

    private Rigidbody2D rb;
    private bool isMovingRight = true;
    private Transform player;

    private Collider2D enemyCollider; // Reference to the Collider2D component

    private bool canTakeDamage = true;  // Flag to control damage cooldown
    public float damageCooldown = 1.0f;  // Cooldown duration in seconds

    public float soundInterval = 5.0f;  // Adjust this value for your desired interval
    private AudioSource audioSource;
    public AudioClip soundClip;

    private void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Get a reference to the Collider2D component
        enemyCollider = GetComponent<Collider2D>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = soundClip;  // Assign your audio clip to the AudioSource
        audioSource.loop = false;     // Ensure the audio doesn't loop
        audioSource.playOnAwake = false; // Prevent automatic playback on awake

    }
    public void PlaySoundFromAnimation()
    {
        if (audioSource != null && soundClip != null)
        {
            audioSource.Play();
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHP > 0 && canTakeDamage)
        {
            currentHP -= damage;

            // Set the "isDamaged" trigger to play the damage animation
            if (animator != null && !string.IsNullOrEmpty(damageAnimationState))
            {
                animator.SetTrigger("isDamaged");
                // Set the "Idle" trigger to transition back to the idle state
                animator.SetTrigger("Idle");
            }

            if (currentHP <= 0)
            {
                Die();
            }
            else
            {
                // Start the cooldown timer
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    private void Die()
    {
        // Disable the Collider2D component to despawn the enemy's collider
        enemyCollider.enabled = false;

        // Disable the Rigidbody2D component to stop any physics interactions
        rb.simulated = false;

        // Set the "isDead" trigger to play the destroy animation
        if (animator != null && !string.IsNullOrEmpty(destroyAnimationState))
        {
            animator.SetTrigger("isDead");
        }

        // Delay a bit before destroying the enemy
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // Wait for the duration of the destroy animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }

    private void Update()
    {
        if (PlayerIsInRange())
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private bool PlayerIsInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRadius;
    }

    private void ChasePlayer()
    {
        if (transform.position.x < player.position.x)
        {
            isMovingRight = true;
        }
        else
        {
            isMovingRight = false;
        }

        // Face the player
        Vector3 scale = transform.localScale;
        if (isMovingRight)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        transform.localScale = scale;

        // Move towards the player
        Vector2 moveDirection = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
    }

    private void Patrol()
    {
        if (isMovingRight)
        {
            if (transform.position.x >= rightPatrolPoint.position.x)
            {
                isMovingRight = false;
                Flip();
            }
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else
        {
            if (transform.position.x <= leftPatrolPoint.position.x)
            {
                isMovingRight = true;
                Flip();
            }
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
    }

    private void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x = isMovingRight ? Mathf.Abs(newScale.x) : -Mathf.Abs(newScale.x);
        transform.localScale = newScale;
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
            }
        }
    }
}
