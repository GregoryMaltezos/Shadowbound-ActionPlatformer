using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Vector2[] patrolCoordinates;
    public float moveSpeed = 3.0f;
    public float detectionRange = 5.0f;

    private int currentPatrolIndex = 0;
    private Transform player;
    private bool isChasingPlayer = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            isChasingPlayer = true;
        }
        else
        {
            isChasingPlayer = false;
        }

        if (isChasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        Vector2 target = patrolCoordinates[currentPatrolIndex];
        Vector2 moveDirection = (target - (Vector2)transform.position).normalized;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolCoordinates.Length;
        }
    }

    private void ChasePlayer()
    {
        Vector2 playerPosition = player.position;
        Vector2 moveDirection = (playerPosition - (Vector2)transform.position).normalized;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}
