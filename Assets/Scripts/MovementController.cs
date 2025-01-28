using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    Vector2 move;
    public int spd;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Flip();
    }

    private void FixedUpdate()
    {
        rb.AddForce(move * spd, ForceMode2D.Force);
    }
    private void Flip()
    {
        /* if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
         {
             isFacingRight = !isFacingRight;
             Vector3 localScale = transform.localScale;
             localScale.x *= -1f;
             transform.localScale = localScale;
         }
     }
      */

    }
}
