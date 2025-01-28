using UnityEngine;

public class ReflectableFireball : MonoBehaviour, IAttackable
{
    public int damage = 1; // Damage inflicted on collision with the player
    private bool isReflectable = false;
    private float reflectSpeed = 7f; // Speed at which the fireball reflects back
    private Vector2 initialDirection;
    private bool reflected = false;
    public GameObject deathSpawnPrefab;

    void Start()
    {
        // Store the initial direction of the fireball
        initialDirection = GetComponent<Rigidbody2D>().velocity.normalized;
    }

    void Update()
    {
        // Check if the fireball is reflectable, moving away from its initial direction, and hasn't been reflected yet
        if (isReflectable && Vector2.Dot(GetComponent<Rigidbody2D>().velocity.normalized, initialDirection) < 0 && !reflected)
        {
            Reflect();
        }
    }

    public void SetReflectable(bool reflectable)
    {
        isReflectable = reflectable;
    }

    public void SetReflectSpeed(float speed)
    {
        reflectSpeed = speed;
    }

    public void SetInitialDirection(Vector2 direction)
    {
        initialDirection = direction;
    }

    void Reflect()
    {
        // Reverse the direction of the fireball
        GetComponent<Rigidbody2D>().velocity = -GetComponent<Rigidbody2D>().velocity.normalized * reflectSpeed;

        // Move the fireball one unit to the right


        // Change the rotation of the fireball by -180 degrees
        transform.Rotate(0f, 0f, -180f);

        Vector3 newPosition = transform.position + Vector3.right;
        transform.position = newPosition;

        // Set the "reflected" flag to true to prevent further reflection
        reflected = true;
    }






    void OnCollisionEnter2D(Collision2D collision)
    {
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
        else if (collision.gameObject.CompareTag("Boss"))
        {
            // Deal damage to the boss when colliding with it
            BossController bossHealth = collision.gameObject.GetComponent<BossController>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }

            // Destroy the fireball on collision with the boss
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

    public void TakeDamage(int damage)
    {
        // Handle the case when the player's attack collides with the fireball
        // For example, you can set the fireball to be reflectable
        SetReflectable(true);
    }



    public bool IsReflectable()
    {
        return isReflectable;
    }


    


}
