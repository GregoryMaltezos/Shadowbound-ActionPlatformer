using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestInteraction : MonoBehaviour
{
    public Animator chestAnimator; // Reference to the chest animator
    public Text interactionText;    // Reference to the UI text for interaction message
    public string openMessage = "";

    private bool isInRange = false;
    private bool isOpen = false;

    public AudioSource chestAudioSource;
    public AudioClip openSound;

    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private PlayerHealth playerHealth;

    void Start()
    {
        interactionText.text = "";
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerAnimator = FindObjectOfType<PlayerMovement>().animator;
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        if (isInRange)
        {
            if (!isOpen && Input.GetKeyDown(KeyCode.E))
            {
                OpenChest();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            interactionText.text = openMessage;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            interactionText.text = "";
        }
    }

    void OpenChest()
    {
        isOpen = true;
        chestAnimator.SetBool("IsOpened", true);

        chestAudioSource.Play();
        // Disable player movement
        playerMovement.enabled = false;

        // Stop the player from flipping
        playerMovement.canFlip = false;

        // Trigger the death animation
        playerAnimator.SetTrigger("Die"); // "Die" is the trigger parameter

        // Display a message or perform other actions when the chest is opened
        interactionText.text = "HAHAHAHAHHAHA YEAH NO, DIE";
        Invoke("RestartGame", 1f);
    }
    void RestartGame()
    {
       
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

}
