using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of platform movement
    public float moveDistance = 5f; // Distance the platform should move
    public float returnSpeed = 2f; // Speed of platform return

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool movingRight = true;

    private void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + Vector3.right * moveDistance;
    }

    private void Update()
    {
        if (movingRight)
        {
            MovePlatform(targetPosition, moveSpeed);
        }
        else
        {
            MovePlatform(originalPosition, returnSpeed);
        }
    }

    private void MovePlatform(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            if (movingRight)
            {
                movingRight = false;
            }
            else
            {
                movingRight = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
