using UnityEngine;

public class KnockBack : MonoBehaviour
{
    public float knockbackForce = 5.0f;
    public float invincibilityDuration = 1.0f;

    private bool isInvincible = false;
    private float invincibilityTimer = 0.0f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(Vector2 damageDirection)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;

            // Apply knockback
            rb.velocity = damageDirection.normalized * knockbackForce;
        }
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;

            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }
}
