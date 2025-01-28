using UnityEngine;

public class CameraControllerVirt : MonoBehaviour
{
    public float verticalSpeed = 5.0f; // Adjust the vertical speed as needed
    public float maxY = 10.0f; // Adjust the maximum Y position
    public float minY = -10.0f; // Adjust the minimum Y position
    public Transform player; // Reference to the player's transform

    private void Update()
    {
        // Move the camera vertically
        float inputVertical = Input.GetAxis("Vertical");
        float newPositionY = transform.position.y + (inputVertical * verticalSpeed * Time.deltaTime);

        // Clamp the new position within the specified bounds
        newPositionY = Mathf.Clamp(newPositionY, minY, maxY);

        transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);
    }
}
