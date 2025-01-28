using UnityEngine;

public class LeverInteraction : MonoBehaviour
{
    public float flipAngle = 45f;
    public int leverNumber; // Assign this in the Inspector
    public AudioClip leverSound; // Assign the sound effect in the Inspector
    private bool isFlipped = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Add an AudioSource component if one doesn't exist
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = leverSound;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    ToggleLeverState();
                }
            }
        }
    }

    void ToggleLeverState()
    {
        if (isFlipped)
        {
            transform.Rotate(Vector3.forward * -flipAngle); // Rotate back by the same angle
            isFlipped = false;
            if (audioSource != null && leverSound != null)
            {
                audioSource.Play();
            }
        }
        else
        {
            transform.Rotate(Vector3.forward * flipAngle); // Rotate by the specified angle
            isFlipped = true;

            // Play the sound effect
            if (audioSource != null && leverSound != null)
            {
                audioSource.Play();
            }
        }
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }
}
