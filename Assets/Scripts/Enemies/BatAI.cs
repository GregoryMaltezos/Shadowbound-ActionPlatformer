using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BatAI : MonoBehaviour, IAttackable
{
    public Transform target;
    public float speed = 200f;
    public float nextWPdistance = 3f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;
    Seeker seeker;
    Rigidbody2D rb;

    public Transform sprite;

    public float detectionRange = 10.0f;

    public int maxHP = 2;
    private int currentHP;

    public Animator animator;
    public string damageAnimationState = "Damage";
    public string destroyAnimationState = "Destroy";

    public int damageAmount = 1;
    private bool isTriggered = false;
    public Vector2 knockbackDirection = new Vector2(1.0f, 1.0f);

    private Transform player;
    private Collider2D enemyCollider;

    private bool canTakeDamage = true;
    public float damageCooldown = 1.0f;

    public AudioSource wingFlapSound;
    public AudioSource hitSound;

    private bool isPlayingSound;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);

        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyCollider = GetComponent<Collider2D>();

        isPlayingSound = false;
    }

    public void TakeDamage(int damage)
    {
        if (currentHP > 0 && canTakeDamage)
        {
            currentHP -= damage;

            if (hitSound != null)
            {
                hitSound.Play();
            }

            if (animator != null && !string.IsNullOrEmpty(damageAnimationState))
            {
                animator.SetTrigger("DamagedTrigger");
                animator.SetTrigger("Idle");
            }

            if (currentHP <= 0)
            {
                Die();
            }
            else
            {
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
        enemyCollider.enabled = false;
        rb.simulated = false;

        if (animator != null && !string.IsNullOrEmpty(destroyAnimationState))
        {
            animator.SetTrigger("isDead");
        }

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            KnockBack knockback = other.GetComponent<KnockBack>();

            if (knockback != null)
            {
                knockback.TakeDamage(knockbackDirection);
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (path == null)
            return;

        float playerDistance = Vector2.Distance(rb.position, player.position);

        if (playerDistance <= detectionRange)
        {
            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWPdistance)
            {
                currentWaypoint++;
            }

            if (force.x > 0.01f)
            {
                sprite.localScale = new Vector3(1f, 1, 1f);
            }
            else if (force.x < -0.01f)
            {
                sprite.localScale = new Vector3(-1f, 1, 1f);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        // Check if the wing flap sound should be played
        if (!isPlayingSound && wingFlapSound != null)
        {
            wingFlapSound.Play();
            isPlayingSound = true;
        }
    }

    public interface IAttackable
    {
        void TakeDamage(int damage);
    }
}
