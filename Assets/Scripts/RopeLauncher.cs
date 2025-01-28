using UnityEngine;

public class RopeLauncher : MonoBehaviour
{
    public GameObject ropePrefab;
    public Transform firePoint;
    public float ropeLength = 5f;

    private GameObject currentRope;
    private LineRenderer ropeRenderer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireRope();
        }
    }

    private void FireRope()
    {
        if (currentRope != null)
        {
            Destroy(currentRope);
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 fireDirection = (mousePos - firePoint.position).normalized;

        currentRope = Instantiate(ropePrefab, firePoint.position, Quaternion.identity);
        ropeRenderer = currentRope.GetComponent<LineRenderer>();

        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, firePoint.position);
        ropeRenderer.SetPosition(1, firePoint.position + (Vector3)fireDirection * ropeLength);
    }
}
