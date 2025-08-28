using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb2d;
    private Animator animator;
    public int PlayerHealth = 90;

    public float speed;
    
    private bool comboQueued;
    
    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public bool isGrounded;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D> ();
        animator = GetComponent<Animator> ();
    }

    void Update()
    {
        CheckAttack();
        HandleJump();
        HandleHorizontalMovement();

    }
    private void HandleHorizontalMovement()
    {
        float moveInput = 0f;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }

        // Apply movement
        rb2d.linearVelocity = new Vector2(moveInput * speed, rb2d.linearVelocity.y);
        if (rb2d.linearVelocity.x > 0.1 || rb2d.linearVelocity.x < -0.1)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        // Flip the character sprite based on direction
        if (moveInput > 0) {
         transform.localScale = new Vector3(6, 6, 1);
        }else if (moveInput < 0)
        {
         transform.localScale = new Vector3(-6, 6, 1);
        }

    }

    private void HandleJump()
    {
        // Check if player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("jumpVelocity", rb2d.linearVelocityY);

        // Jump when X is pressed and player is grounded
        if (Input.GetKeyDown(KeyCode.X) && isGrounded)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
        }
    }

    private void CheckAttack()
    {  
            if (Input.GetKeyDown(KeyCode.Z)) // Left mouse for attack
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1"))
                {
                    comboQueued = true;
                }
                else if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    // Start first attack if not attacking
                    animator.SetTrigger("Attack");
                }
            }
    }

    public void CheckCombo()
    {
        if (comboQueued)
        {
            animator.SetBool("Combo", true);
            comboQueued = false;
        }
        else
        {
            animator.SetBool("Combo", false);
        }
    }
}
