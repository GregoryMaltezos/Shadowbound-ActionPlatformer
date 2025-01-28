using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private float horizontal;
    public bool canFlip = true;


    Rigidbody2D rb;
    Vector2 move;
    public int spd;

    public Animator animator;

    [Header("Jump System")]
    [SerializeField] float jumpTime;
    [SerializeField] int jumpPower;
    [SerializeField] float fallMult;
    [SerializeField] float jumpMult;

    float h;
    bool jump;


    [Header("Wall Jump System")]
    public Transform wallCheck;
    public LayerMask wallLayer;
    bool isTouchingWall;
    bool isSliding;
    bool isGrounded;
    public float wallSlideSpeed;
    public float wallJumpDuration;
    public Vector2 wallJumpForce;
    bool wallJumping;


    public Transform groundCheck;
    public LayerMask groundLayer;


    private bool canDash = true;
    private bool isDashing;
    private float dashPower = 50f;
    private float dashTime = 0.2f;
    private float dashCD = 1f;
    [SerializeField] private TrailRenderer tr;

    
    Vector2 vecGravity;

    bool isJumping;
    float jumpCount;

    public AudioSource dashAudioSource;
    public AudioClip dashSound;

    Vector2 vecMove;
    public AudioSource jumpAudioSource; 
    public AudioClip jumpSound;

    public AudioSource walkAudioSource;
    public AudioClip walkSound;


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        tr.emitting = true;

        // Set the dash sound clip and play it
        dashAudioSource.clip = dashSound;
        dashAudioSource.Play();

        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }




    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        dashAudioSource = GetComponent<AudioSource>();
        dashAudioSource.clip = dashSound;
        jumpAudioSource = gameObject.AddComponent<AudioSource>(); // Create a new AudioSource component
        jumpAudioSource.clip = jumpSound;
        walkAudioSource = gameObject.AddComponent<AudioSource>(); 
        walkAudioSource.clip = walkSound;
        walkAudioSource.loop = true;
    }


    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
        }
        else if (isSliding)
        {
            wallJumping = true;
            Invoke("StopWallJump", wallJumpDuration);
        }
        jump = false;
    }

    
    void StopWallJump()
    {
        wallJumping = false;
    }



    public void Jump(InputAction.CallbackContext value)
    {
        if (value.started && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            jumpCount = 0;
            jumpAudioSource.Play();
        }
        if (value.canceled)
        {
            isJumping = false;
        }
    }

    public void Movement(InputAction.CallbackContext value)
    {
        vecMove = value.ReadValue<Vector2>();
        Flip();
    }



    private void Update()
    {

        if (!playerHealth.IsAlive) // Check the player's status using the reference to PlayerHealth
        {
            return; // Don't process movement if the player is dead
        }

        if (isDashing)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (jump && IsGrounded())
        {
            dashAudioSource.Play();
        }
    

    h = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        if (IsWalled() && !IsGrounded() && h != 0)
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed",Mathf.Abs(horizontal));

    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        // Handle horizontal movement
        rb.velocity = new Vector2(vecMove.x * spd, rb.velocity.y);

        // Apply gravity and jump behavior
        if (rb.velocity.y < 0)
        {
            rb.velocity -= vecGravity * fallMult * Time.deltaTime;
        }
        if (rb.velocity.y > 0 && isJumping)
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

        // Handle jumping
        if (jump)
        {
            Jump();
        }


        isGrounded = IsGrounded();

        // Play the walking sound when on the ground and moving
        if (isGrounded && Mathf.Abs(rb.velocity.x) > 0.01f)
        {
            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
        }
        else
        {
            walkAudioSource.Stop();
        }


        // Apply wall slide speed only when actively sliding on a wall
        if (isSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }

        // Apply a constant gravity force when not grounded and not sliding on a wall
        if (!isGrounded && !isSliding)
        {
            rb.velocity -= vecGravity * Time.deltaTime;
        }

        // Handle wall jumping
        if (wallJumping)
        {
            jumpAudioSource.Play();
            rb.velocity = new Vector2(-h * wallJumpForce.x, wallJumpForce.y);

            // Add a force to slow down horizontal movement after a wall jump
            rb.velocity += Vector2.right * -h * wallJumpForce.x * 0.5f;
        }
        else
        {
            rb.velocity = new Vector2(h * spd, rb.velocity.y);
        }
    }




    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(4f, 0.4f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCapsule(wallCheck.position, new Vector2(0.3f, 3.56f), CapsuleDirection2D.Vertical, 0, wallLayer);
    }


    void Flip()
    {
        if (!playerHealth.IsAlive || !canFlip) // Check the player's status
        {
            return; // Don't flip the character if the player is dead
        }

        if (vecMove.x < -0.01f)
        {
            transform.localScale = new Vector3(-0.2f, 0.2f, 0.2f);
        }
        if (vecMove.x > 0.01f)
        {
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }

}
    