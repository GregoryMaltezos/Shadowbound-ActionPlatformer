using UnityEngine;

public class DeathPrefabFaller : MonoBehaviour
{
    public float fallSpeed = 0.2f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0f, -fallSpeed);
    }
}
