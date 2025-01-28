using System.Collections;
using UnityEngine;

public class SpikeTrapMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float moveDistance = 5.0f;
    public float delayTime = 2.0f;

    private Vector3 initialPosition;
    private bool movingRight = true;
    private AudioSource audioSource; // Add this line
    public AudioClip returnSound;   // Add this line

    void Start()
    {
        initialPosition = transform.position;
        audioSource = GetComponent<AudioSource>(); // Add this line
        audioSource.clip = returnSound;            // Add this line
        StartCoroutine(MoveTrap());
    }

    IEnumerator MoveTrap()
    {
        while (true)
        {
            if (movingRight)
            {
                Vector3 targetPosition = initialPosition + Vector3.right * moveDistance;
                while (transform.position != targetPosition)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                while (transform.position != initialPosition)
                {
                    transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);
                    yield return null;
                }

                // Play the sound when the spikes return to their initial position.
                if (audioSource != null && returnSound != null)
                {
                    audioSource.Play();
                }
            }

            movingRight = !movingRight;
            yield return new WaitForSeconds(delayTime);
        }
    }
}
