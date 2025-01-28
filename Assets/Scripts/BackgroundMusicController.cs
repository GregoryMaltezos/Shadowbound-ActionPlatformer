using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip backgroundMusic;

    void Start()
    {
        audioSource = GetComponent < AudioSource>();

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // Set to true to loop the background music.
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No background music assigned to the BackgroundMusicController.");
        }
    }
}
