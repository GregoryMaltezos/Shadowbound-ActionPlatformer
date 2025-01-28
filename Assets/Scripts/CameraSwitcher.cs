using System.Collections;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera originalCamera; // Reference to the original camera
    public Camera bossCamera;     // Reference to the boss camera
    public float zoomedInSize = 5f; // The orthographic size when zoomed in
    public float zoomDuration = 1.0f; // Duration for zooming in
    public float switchDelay = 2f; // The delay before switching back to the original camera

    private float originalSize;   // Store the original orthographic size

    private void Start()
    {
        originalSize = originalCamera.orthographicSize;
    }

    public void SwitchToOriginalCamera()
    {
        originalCamera.gameObject.SetActive(true);
        originalCamera.orthographicSize = originalSize;
        bossCamera.gameObject.SetActive(false);
    }

    public void SwitchToBossCamera()
    {
        originalCamera.gameObject.SetActive(false);
        bossCamera.gameObject.SetActive(true);

        StartCoroutine(ZoomInBossCamera());
    }

    private IEnumerator ZoomInBossCamera()
    {
        float elapsedTime = 0;
        float startSize = originalCamera.orthographicSize;
        float targetSize = zoomedInSize;

        while (elapsedTime < zoomDuration)
        {
            bossCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bossCamera.orthographicSize = targetSize; // Ensure it reaches the target size

        StartCoroutine(SwitchBackAfterDelay());
    }

    private IEnumerator SwitchBackAfterDelay()
    {
        yield return new WaitForSeconds(switchDelay);

        // Switch back to the original camera
        SwitchToOriginalCamera();
    }
}