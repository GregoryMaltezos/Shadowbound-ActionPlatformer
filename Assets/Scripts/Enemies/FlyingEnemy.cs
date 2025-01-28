using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float patrolRange = 10f; // Distance to patrol left and right
    public float patrolSpeed = 2f;
    public float detectionRadius = 5f;
    public float attackSpeed = 5f;
    public float attackDescentSpeed = -2f; // Speed to descend towards the player
    public float attackAscendSpeed = 2f; // Speed to ascend back up
    public float attackCooldown = 2f; // Cooldown time between attacks
    public int damageAmount = 10;

    private Transform player;
    private Vector2 leftPatrolPos;
    private Vector2 rightPatrolPos;
    private bool isPatrolling = true;
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 patrolDirection = Vector2.right;

    private Vector2 initialPosition; // Store the initial patrol position

    public CircleCollider2D attackTrigger; // Attach the trigger area here

    void Start()
    {
        rb = GetComponent < Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent <SpriteRenderer>();

        // Calculate left and right patrol positions
        leftPatrolPos = transform.position - Vector3.right * patrolRange / 2f;
        rightPatrolPos = transform.position + Vector3.right * patrolRange / 2f;

        // Store the initial patrol position
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isPatrolling)
        {
            Patrol();
        }
        else if (isAttacking)
        {
            Attack();
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    void Patrol()
    {
        // Move left and right within the patrol range
        Vector2 targetPosition = isMovingLeft() ? leftPatrolPos : rightPatrolPos;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(direction.x * patrolSpeed, rb.velocity.y);

        // Detect the player
        if (Vector2.Distance(transform.position, player.position) < detectionRadius)
        {
            isPatrolling = false;
            TryAttack();
        }

        // Flip sprite based on patrol direction
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // Facing right
        }
        else
        {
            spriteRenderer.flipX = true; // Facing left
        }

        // Check if the enemy should change direction
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            patrolDirection = isMovingLeft() ? Vector2.right : Vector2.left;
        }
    }

    bool isMovingLeft()
    {
        return patrolDirection == Vector2.left;
    }

    void TryAttack()
    {
        if (attackCooldownTimer <= 0)
        {
            isAttacking = true;
            attackCooldownTimer = attackCooldown;
        }
    }

    void Attack()
    {
        if (Vector2.Distance(transform.position, player.position) < 1.0f)
        {
            // Inflict damage on the player using the trigger area
            isAttacking = false;
            PatrolTo(initialPosition);
            rb.velocity = Vector2.zero; // Reset velocity to stop any movement
        }
        else
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float horizontalVelocity = directionToPlayer.x * attackSpeed;
            float verticalVelocity = directionToPlayer.y * attackSpeed;

            // Set the enemy's velocity with separate horizontal and vertical components
            rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);

            // Flip sprite based on the attack direction
            if (horizontalVelocity > 0)
            {
                spriteRenderer.flipX = false; // Facing right
            }
            else
            {
                spriteRenderer.flipX = true; // Facing left
            }
        }
    }

    void PatrolTo(Vector2 position)
    {
        Vector2 direction = (position - (Vector2)transform.position).normalized;
        // Set vertical velocity to ascend back up to the original patrol position
        rb.velocity = new Vector2(direction.x * patrolSpeed, attackAscendSpeed);
    }

    // OnTriggerEnter2D method for the attack trigger area
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking && other.CompareTag("Player"))
        {
            // Inflict damage on the player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
