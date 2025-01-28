using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Animator respawnEffectAnimator;
    private Rigidbody2D playerRigidbody;
    private bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
    }

    public Transform spawnEffectLocation;
    private bool isDead = false;

    public int maxHealth = 3;
    private int currentHealth;
    public float damageFlashDuration = 0.2f;
    public Image[] healthIcons;
    public Color flashColor = Color.red;
    public GameObject characterPrefab;
    private Animator playerAnimator;
    private PlayerMovement playerMovement;

    public GameObject bossObject;


    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private List<Color> originalColors = new List<Color>();
    private bool isInvulnerable = false;

    private RespawnManager respawnManager; // Reference to the RespawnManager

    public GameObject respawnEffect; // Reference to the respawn effect object

    private Vector3 deathLocation; // Store the player's death location

    // Reference to the SpawnPrefabOnTrigger script
    private SpawnPrefabOnTrigger gateSpawner;

    // Define a delegate and an event for player death
    public delegate void PlayerDeathEvent(Vector3 deathLocation);

    public static event PlayerDeathEvent OnPlayerDeath;

    public float respawnDelay = 1.5f; // Respawn delay in seconds

    public AudioSource hitSound;
    public float deathHeight = -10.0f;

    public BossController bossController;
    public bool bossWasActive = false;
    public float bossDisableDelay = 2.0f;

    private void Start()
    {
        currentHealth = maxHealth;
        FindSpriteRenderers(characterPrefab.transform);
        originalColors = spriteRenderers.ConvertAll(sr => sr.color);
        PlayerHealth.OnPlayerDeath += ResetBossOnPlayerDeath;
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent <PlayerMovement>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        // Find and store a reference to the RespawnManager in the scene
        respawnManager = FindObjectOfType<RespawnManager>();
        OnPlayerDeath += ResetBossOnPlayerDeath;
        // Find and store a reference to the SpawnPrefabOnTrigger script
        gateSpawner = FindObjectOfType<SpawnPrefabOnTrigger>();

        // Deactivate the respawn effect at the start
        if (respawnEffect != null)
        {
            respawnEffect.SetActive(false);
        }

        if (hitSound != null)
        {
            hitSound.Stop();
        }
        UpdateUI();
    }
    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        OnPlayerDeath -= ResetBossOnPlayerDeath;
    }

    private void FindSpriteRenderers(Transform transform)
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderers.Add(spriteRenderer);
            }
            FindSpriteRenderers(child);
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (i < currentHealth)
            {
                healthIcons[i].enabled = true;
            }
            else
            {
                healthIcons[i].enabled = false;
            }
        }
    }
    private void RespawnPlayer()
    {
        // Store the player's death location
        deathLocation = transform.position;

        // Freeze the X constraint when respawning
        playerRigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX;


        // Raise the OnPlayerDeath event before respawning
        if (OnPlayerDeath != null)
        {
            OnPlayerDeath(deathLocation);
        }

        // Delay the respawn using a Coroutine
        StartCoroutine(RespawnDelayCoroutine());
        playerAnimator.SetBool("IsDead", false);
        StartCoroutine(UnfreezeXPosition());
    }

    private IEnumerator UnfreezeXPosition()
    {
        yield return new WaitForSeconds(respawnDelay); // Adjust this delay if necessary

        // Unfreeze the X position after the delay
        playerRigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
    }

    private IEnumerator RespawnDelayCoroutine()
    {
        // Wait for the specified respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Set the player's position to the respawn point using the RespawnManager instance
        if (respawnManager != null)
        {
            respawnManager.RespawnPlayer(transform);
        }

        // Reset health and any other player-specific settings 
        currentHealth = maxHealth;
        isAlive = true;

        // Optionally, enable player controls
        playerMovement.enabled = true;

        // Ensure the "Spawn" trigger is set to transition out of the death animation
        playerAnimator.SetTrigger("Spawn");

        // Activate the spawn effect
        if (respawnEffect != null)
        {
            respawnEffect.SetActive(true);
        }

        // Play external animation here
        if (respawnEffectAnimator != null)
        {
            respawnEffectAnimator.SetTrigger("ExternalSpawn");
        }

        // Call the DestroyPrefab method from the gateSpawner script
        if (gateSpawner != null)
        {
            gateSpawner.DestroyPrefab();
        }

        // Update UI after respawn
        UpdateUI();
    }


    private void Update()
    {
        // Check if the player has fallen below the specified death height
        if (transform.position.y < deathHeight)
        {
            if (isAlive)
            {
                // Player has fallen below the death height, trigger death
                isAlive = false;
                playerMovement.enabled = false;

                // Raise the OnPlayerDeath event before respawning
                if (OnPlayerDeath != null)
                {
                    OnPlayerDeath(deathLocation);
                }

                // Respawn the player after a delay (e.g., 1.5 seconds)
                RespawnPlayer();
                playerAnimator.ResetTrigger("Spawn");
            }
        }
    }




    public void TakeDamage(int damage)
    {
        if (!isInvulnerable)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
            if (hitSound != null)
            {
                hitSound.Play();
            }

            if (currentHealth > 0)
            {
                StartCoroutine(FlashOnDamage());

            }
            else
            {
                StartCoroutine(FlashOnDamage());
                Debug.Log("Player has died");
                isAlive = false;
                isInvulnerable = true;
                // Trigger the death animation
                playerAnimator.SetTrigger("DieTrigger");

                // Disable player movement
                playerMovement.enabled = false;
                playerRigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                // Respawn the player after a delay (e.g., 1.5 seconds)
                RespawnPlayer();
                playerAnimator.ResetTrigger("Spawn");
                isInvulnerable = false;
            }

            UpdateUI();
        }
    }

    private void ResetBossOnPlayerDeath(Vector3 deathLocation)
    {
        if (this != null)
        {
            StartCoroutine(ResetBossWithDelay(deathLocation));
        }
    }

    private IEnumerator ResetBossWithDelay(Vector3 deathLocation)
    {
        if (bossObject != null)
        {
            float distance = Vector3.Distance(bossObject.transform.position, deathLocation);

            if (distance < 25f)
            {
                BossController bossController = bossObject.GetComponent<BossController>();
                if (bossController != null)
                {
                    yield return new WaitForSeconds(bossDisableDelay);
                    bossController.ResetBoss(bossObject);
                }

                bossObject.SetActive(false);
            }
        }
    }







    private IEnumerator FlashOnDamage()
    {
        isInvulnerable = true;
        Debug.Log("Player is invulnerable");

        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].color = flashColor;
        }

        yield return new WaitForSeconds(damageFlashDuration);

        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }

        isInvulnerable = false;
        Debug.Log("Player is no longer invulnerable");
    }
}
