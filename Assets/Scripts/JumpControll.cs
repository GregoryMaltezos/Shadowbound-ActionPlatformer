using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpControll : MonoBehaviour
{
    Rigidbody2D rb;


    [SerializeField] float jumpTime;
    [SerializeField] int jumpPower;
    [SerializeField] float fallMult;
    [SerializeField] float jumpMult;

    public Transform groundCheck;
    public LayerMask groundLayer;
    Vector2 vecGravity;

    bool isJumping;
    float jumpCount;

    // Start is called before the first frame update
    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rb = GetComponent<Rigidbody2D>();
    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(4f, 0.4f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            jumpCount = 0;
        }

        if(rb.velocity.y>0 && isJumping)
        { 
            jumpCount += Time.deltaTime;
            if (jumpCount > jumpTime)
            {
                isJumping = false;
            }
            float t = jumpCount / jumpTime;
            float currentJumpM = jumpMult;
            if (t > 0.5f)
            {
                currentJumpM = jumpMult * (1 - t);
            }
           
            rb.velocity += vecGravity * currentJumpM * Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }


        if (rb.velocity.y < 0)
        {
            rb.velocity -= vecGravity * fallMult * Time.deltaTime;
        }
    }
}
