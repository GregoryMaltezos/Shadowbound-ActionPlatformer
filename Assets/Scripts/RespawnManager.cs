using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private Vector3 respawnPoint = Vector3.zero; // Remove the 'static' keyword

    public void SetRespawnPoint(Vector3 point)
    {
        respawnPoint = point;
    }

    public void RespawnPlayer(Transform playerTransform)
    {
        playerTransform.position = respawnPoint;
    }
}
