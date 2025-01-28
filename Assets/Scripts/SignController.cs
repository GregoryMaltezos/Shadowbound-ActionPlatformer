using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignController : MonoBehaviour
{
    [SerializeField] // Make the field visible in the Inspector.
    private Canvas popupCanvas; // Reference to the Canvas displaying the pop-up.
    public float triggerDistance = 2f; // The distance at which the pop-up should appear.

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Make sure to tag your player GameObject as "Player".
        popupCanvas.gameObject.SetActive(false); // Hide the Canvas initially.
    }

    private void Update()
    {
        // Check the distance between the sign and the player.
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= triggerDistance)
        {
            ShowPopUp();
        }
        else
        {
            HidePopUp();
        }
    }

    void ShowPopUp()
    {
        popupCanvas.gameObject.SetActive(true);
    }

    void HidePopUp()
    {
        popupCanvas.gameObject.SetActive(false);
    }
}
