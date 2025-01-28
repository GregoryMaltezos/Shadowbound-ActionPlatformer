using UnityEngine;

public class EquipKnifeInteract : MonoBehaviour
{
    public GameObject subSubSubPrefab; // Reference to the sub-sub-sub prefab.
    public float interactionDistance = 2f; // The maximum distance for interaction.
    private PlayerStatus playerStatus;

    private bool hasInteracted = false; // Flag to track if the interaction has occurred;

    private void Start()
    {
        // Get the PlayerStatus component on the player character
        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !hasInteracted)
        {
            // Find the player GameObject
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                // Calculate the distance between the player and the GameObject.
                float distance = Vector3.Distance(player.transform.position, transform.position);

                if (distance <= interactionDistance)
                {
                    // Activate the sub-sub-sub prefab.
                    subSubSubPrefab.SetActive(true);

                    // Deactivate the floating item (sub-sub-sub prefab).
                    gameObject.SetActive(false);

                    // Update the hasWeapon status in the PlayerStatus script
                    playerStatus.AcquireWeapon();

                    hasInteracted = true;
                }
            }
        }
    }
}
