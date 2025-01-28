using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image[] healthIcons; // An array to store the health icons.
    public PlayerHealth playerHealth; // Reference to the PlayerHealth script.

    private void Start()
    {
        // Get the PlayerHealth script on the player.
        playerHealth = GetComponent<PlayerHealth>();

        // Initialize the health icons.
        healthIcons = new Image[3]; // Assuming you have 3 health icons.

        // Assign the UI Image components for the health icons (you should link them in the Inspector).
        healthIcons[0] = transform.Find("HealthIcon1").GetComponent<Image>();
        healthIcons[1] = transform.Find("HealthIcon2").GetComponent<Image>();
        healthIcons[2] = transform.Find("HealthIcon3").GetComponent<Image>();
    }
/*
 * 
    private void Update()
    {
        // Update the health icons based on the player's current health.
        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (i < playerHealth.currentHealth)
            {
                // Enable the health icon if the player's current health is greater or equal to its index.
                healthIcons[i].enabled = true;
            }
            else
            {
                // Disable the health icon if the player's current health is less than its index.
                healthIcons[i].enabled = false;
            }
        }
    }*/
}
