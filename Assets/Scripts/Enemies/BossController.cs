using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{


    // Variables for fireballs and attack parameters
    public GameObject fireballPrefab;
    public GameObject reflectableFireballPrefab;
    public float fireballSpeed = 5f;
    public int numFireballs = 5;
    public float spreadAngle = 45f;
    public float fireRate = 2f;
    public Transform firePoint;
    public float reflectableFireballOffset = 1.0f;
    private float nextFireTime;
    private int reflectableIndex;
    public float[] initialRotations;
    private Animator bossAnimator;
    public float attackAnimationDuration = 4.0f;
    private bool isAttacking = false;
    public int maxBossHP = 5;
    private int currentBossHP;
    public Slider bossHealthBar; // Reference to the Boss Health Bar UI
    private SpriteRenderer bossRenderer;
    public float hitFlashDuration = 0.2f;
    private bool isHit = false;
    private Color originalColor;
    public GameObject deathSpawnPrefab;
    private AudioSource audioSource;
    public AudioClip fireballSound;
    public AudioClip bossGrowlSound;
    public AudioClip bossHitSound;
    private bool bossActive = false;
    public GameObject chestObject;

    void Start()
    {
        // Initializing boss properties and components
        reflectableIndex = Random.Range(0, numFireballs);
        initialRotations = new float[numFireballs];
        initialRotations[0] = 253.0f;
        initialRotations[1] = 258.0f;
        initialRotations[2] = 270.0f;
        initialRotations[3] = 283.0f;
        initialRotations[4] = 289.0f;

        bossAnimator = GetComponent<Animator>();
        bossRenderer = GetComponent<SpriteRenderer>();
        originalColor = bossRenderer.color; // Store the original color
        currentBossHP = maxBossHP;
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }

    void Update()
    {
        // Controls boss attacks based on health and animation state
        if (currentBossHP > 0 && !isAttacking)
        {
            if (Time.time > nextFireTime)
            {
                isAttacking = true;
                bossAnimator.SetTrigger("IsAttacking");
                StartCoroutine(PerformAttackAfterAnimation());
                nextFireTime = Time.time + fireRate;
            }
        }
    }


    IEnumerator PerformAttackAfterAnimation()
    {
        // Delays attack after an animation is complete
        yield return new WaitForSeconds(attackAnimationDuration);

        if (currentBossHP > 0) // Check if the boss is still alive
        {
            ShootFireballs();
            bossAnimator.SetTrigger("ReturnToIdle");
        }

        isAttacking = false;
    }

    void ShootFireballs()
    {
        // Logic to shoot fireballs in various directions
        reflectableIndex = Random.Range(0, numFireballs);

        for (int i = 0; i < numFireballs; i++)
        {
            float initialRotation = initialRotations[i];
            Quaternion initialRotationQuat = Quaternion.Euler(0, 0, initialRotation);

            float rotationAngle = -spreadAngle / 2 + i * (spreadAngle / (numFireballs - 1));
            Quaternion rotation = Quaternion.Euler(0, 0, 180 + rotationAngle);
            Vector2 fireballDirection = rotation * Vector2.right;
            Vector2 fireballPosition = (Vector2)firePoint.position + fireballDirection * (i == reflectableIndex ? reflectableFireballOffset : 0);

            GameObject newFireballObj;
            if (i == reflectableIndex)
            {
                newFireballObj = Instantiate(reflectableFireballPrefab, fireballPosition, initialRotationQuat);
                ReflectableFireball reflectableFireball = newFireballObj.GetComponent<ReflectableFireball>();
                reflectableFireball.SetReflectable(true);
                reflectableFireball.SetInitialDirection(fireballDirection.normalized);
                if (audioSource != null && fireballSound != null)
                {
                    audioSource.clip = fireballSound;
                    audioSource.Play();
                }
            }
            else
            {
                newFireballObj = Instantiate(fireballPrefab, fireballPosition, initialRotationQuat);
                if (audioSource != null && fireballSound != null)
                {
                    audioSource.clip = fireballSound;
                    audioSource.Play();
                }
            }

            Rigidbody2D rb = newFireballObj.GetComponent<Rigidbody2D>();
            rb.velocity = fireballDirection.normalized * fireballSpeed;

            if (!(i == reflectableIndex && newFireballObj.CompareTag("ReflectableFireball") && !newFireballObj.CompareTag("Reflected")))
            {
                Destroy(newFireballObj, 6.0f); // 6 seconds delay before destroying
            }
        }
    }

    public void ReflectFireball(Vector2 direction)
    {
        // Logic for reflecting a fireball
        GameObject newFireball = Instantiate(reflectableFireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = newFireball.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * fireballSpeed;
    }

    public void TakeDamage(int damage)
    {
        // Logic to process damage, change boss appearance, and handle death
        if (!isHit)
        {
            isHit = true;
            bossRenderer.color = Color.red; // Flash white when hit
            Invoke("ResetHit", hitFlashDuration);
            if (audioSource != null && bossHitSound != null)
            {
                audioSource.clip = bossHitSound;
                audioSource.Play();
            }
            currentBossHP -= damage;
            bossHealthBar.value = (float)currentBossHP / maxBossHP;

            if (currentBossHP <= 0)
            {
                bossAnimator.SetTrigger("Death"); // Play death animation
                Destroy(gameObject, bossAnimator.GetCurrentAnimatorStateInfo(0).length);
                GameObject deathSpawnInstance = Instantiate(deathSpawnPrefab, transform.position, Quaternion.identity);
                Destroy(deathSpawnInstance, 1.9f);
                ActivateChest();

            }
        }
    }



    private void ActivateChest()
    {
        chestObject.SetActive(true);
    }


    public void ResetBoss(GameObject bossObj)
    {
        // Reset boss properties or other functionalities here and activates boss again
        currentBossHP = maxBossHP;
        bossHealthBar.value = 1.0f;

        isHit = false;

        if (bossRenderer != null)
        {
            bossRenderer.color = originalColor;
        }

        // Reset any other boss-specific properties or behavior

        // Reset the next fire time to allow the boss to attack again immediately
        nextFireTime = Time.time + fireRate;

        // Ensure bossObj is valid before accessing its properties or components
        if (bossObj != null)
        {
            BossController bossController = bossObj.GetComponent<BossController>();
            if (bossController != null)
            {
                bossController.SetBossActive(true); // Reactivate the boss
            }

            // Activate or deactivate the boss GameObject as needed
            bossObj.SetActive(true);
            bossAnimator.SetTrigger("ReturnToIdle");
            isAttacking = false;
        }
    }



    public void SetBossActive(bool active)
    {
        bossActive = active;

        // Toggle the boss's GameObject and Collider
        gameObject.SetActive(active);

        if (active && !isAttacking && currentBossHP > 0)
        {
            StartCoroutine(PerformAttackAfterDelay()); // Start the attack sequence
        }
    }

    IEnumerator PerformAttackAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // Delay before starting attack

        if (currentBossHP > 0)
        {
            isAttacking = true;
            bossAnimator.SetTrigger("IsAttacking");
            StartCoroutine(PerformAttackAfterAnimation());
            nextFireTime = Time.time + fireRate;
        }
    }
    void PlayBossGrowl()
        {
            if (audioSource != null && bossGrowlSound != null)
            {
                audioSource.clip = bossGrowlSound;
                audioSource.Play();
            }
        }


        private void ResetHit()
        {
            isHit = false;
            bossRenderer.color = originalColor; // Reset to the original color
        }
    }