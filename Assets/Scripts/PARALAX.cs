using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PARALAX : MonoBehaviour
{
    public float parallaxSpeed = 0.5f;
    public Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private float backgroundWidth;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;

        // Calculate the width of the background.
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float parallaxOffset = (previousCameraPosition.x - cameraTransform.position.x) * parallaxSpeed;

        transform.position = new Vector3(transform.position.x + parallaxOffset, transform.position.y, transform.position.z);

        // Check if the background has moved too far in one direction and reset its position.
        if (Mathf.Abs(transform.position.x - cameraTransform.position.x) >= backgroundWidth)
        {
            // Calculate the new background position to create a looping effect.
            float offset = Mathf.Sign(parallaxOffset) * backgroundWidth;
            transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
        }

        previousCameraPosition = cameraTransform.position;
    }
}
