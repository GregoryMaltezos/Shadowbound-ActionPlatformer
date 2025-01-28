using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject grapplingHookPrefab;
    public float hookSpeed = 10f;
    public float maxDistance = 10f;
    public LineRenderer ropeRenderer; // Reference to the Line Renderer component

    private GameObject currentHook;
    private bool isGrappling = false;
    private PlayerStatus playerStatus;
    public AudioSource dashAudioSource;

    public AudioSource grapplingHookAudioSource;
    public AudioClip grapplingHookSound;

    private void Start()
    {
        // Get the PlayerStatus component on the player character
        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isGrappling && playerStatus.hasGrapple)
        {
            FireGrapplingHook();
        }

        if (Input.GetKeyUp(KeyCode.F) && isGrappling)
        {
            StopGrapplingHook();
        }

        if (isGrappling)
        {
            HandleGrappling();
        }
    }

    private void FireGrapplingHook()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 fireDirection = (mousePosition - transform.position).normalized;

        // Create a layer mask to filter collisions with the "GrapplePoint" layer
        int grappleLayerMask = LayerMask.GetMask("grapplePoint");
        int blockingLayerMask = LayerMask.GetMask("Wall", "Ground");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, fireDirection, maxDistance, grappleLayerMask);

        if (hit.collider != null)
        {
            // Check if there is a clear path between the player and the grapple point
            bool isClearPath = !Physics2D.Linecast(transform.position, hit.point, blockingLayerMask);

            if (isClearPath)
            {
                isGrappling = true;
                // Play the grappling hook sound
                dashAudioSource.Stop();
                grapplingHookAudioSource.clip = grapplingHookSound;
                grapplingHookAudioSource.Play();

                // Instantiate the grappling hook prefab at the hit point
                currentHook = Instantiate(grapplingHookPrefab, hit.point, Quaternion.identity);

                // Calculate the rotation to align the grappling hook with the rope
                Vector2 direction = hit.point - (Vector2)transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                currentHook.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Set the ropeRenderer positions
                ropeRenderer.positionCount = 2; // 2 vertices: player and hook

                // Set the player's position as the starting point (index 0)
                ropeRenderer.SetPosition(0, transform.position);

                // Set the desired endpoint position relative to the player's position (x = -9.855, y = 2.863)
                Vector3 ropeEndpoint = new Vector3(-9.855f, 2.863f, 0f);
                ropeRenderer.SetPosition(1, ropeEndpoint);
            }
        }
    }

    private void StopGrapplingHook()
    {
        if (currentHook != null)
        {
            Destroy(currentHook);
            ropeRenderer.positionCount = 0; // Clear the rope renderer
        }
        isGrappling = false;
    }

    private void HandleGrappling()
    {
        if (currentHook != null)
        {
            Vector3 hookPosition = currentHook.transform.position;
            Vector3 currentPosition = transform.position;

            float distance = Vector3.Distance(hookPosition, currentPosition);

            // Check if the player has reached the hook
            if (distance <= 0.2f)
            {
                StopGrapplingHook(); // Stop grappling when the player reaches the hook
            }
            else
            {
                // Update the rope positions with interpolation
                ropeRenderer.SetPosition(0, transform.position); // Update player's position
                ropeRenderer.SetPosition(1, hookPosition); // Update hook position

                // Move the character towards the hook
                transform.position = Vector3.MoveTowards(currentPosition, hookPosition, hookSpeed * Time.deltaTime);
            }
        }
        else
        {
            isGrappling = false;
        }
    }
}
