using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public Animator bossAnimator; // Reference to the Boss's Animator component
    public GameObject bossObject; // Reference to the Boss game object
    public CameraSwitcher cameraSwitcher; // Reference to the CameraSwitcher script

    private bool bossActivated = false;
    private bool bossPreviouslyActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!bossActivated)
            {
                ActivateBoss();
            }
            else
            {
                if (!bossPreviouslyActivated)
                {
                    ActivateBoss();
                }
            }
        }
    }

    private void ActivateBoss()
    {
        bossActivated = true;
        bossPreviouslyActivated = true; // Boss has been activated

        cameraSwitcher.SwitchToBossCamera();

        if (bossObject != null)
        {
            bossObject.SetActive(true);
            // Add any code to make the boss attack here
        }
    }

    // Subscribe to the OnPlayerDeath event
    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += ResetBossOnPlayerDeath;
    }

    // Unsubscribe from the event when the script is disabled
    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= ResetBossOnPlayerDeath;
    }

    private void ResetBossOnPlayerDeath(Vector3 deathLocation)
    {
        // Reset the boss activation flags when the player dies
        bossActivated = false;
        bossPreviouslyActivated = false;

        if (bossObject != null)
        {
            bossObject.SetActive(false); // Disable the boss on player death
        }
    }


private void DeactivateBoss()
    {
        bossActivated = false;
        bossPreviouslyActivated = true;
        cameraSwitcher.SwitchToOriginalCamera();

        if (bossObject != null)
        {
            bossObject.SetActive(false);
        }
    }

    public void SwitchToOriginalCamera()
    {
        cameraSwitcher.SwitchToOriginalCamera();
    }
}
