using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb2d;
    private Animator animator;
    public bool canMove = true;
    public int PlayerHealth = 90;

    public float speed;
    
    private bool comboQueued;
    
    [Header("Horizontal Movement")]
    public float HorizontalmoveInput = 0f;
    
    
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public bool isGrounded;

    void Start()
    {
        canMove = true;
        rb2d = GetComponent<Rigidbody2D> ();
        animator = GetComponent<Animator> ();
    }

    void Update()
    {
        checkIfGrounded();
        if (canMove)
        {
            CheckAttack();
            HandleJump();
            HandleHorizontalMovement();
        }

    }
    private void HandleHorizontalMovement()
    {

        if (HorizontalmoveInput == 0)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                HorizontalmoveInput = -1f;
            } else if (Input.GetKey(KeyCode.RightArrow))
            {
                HorizontalmoveInput = 1f;
            }
        }

        // Apply movement
        rb2d.linearVelocity = new Vector2(HorizontalmoveInput * speed, rb2d.linearVelocity.y);
        if (rb2d.linearVelocity.x > 0.1 || rb2d.linearVelocity.x < -0.1)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        // Flip the character sprite based on direction
        if (HorizontalmoveInput > 0) {
         transform.localScale = new Vector3(5, 5, 1);
        }else if (HorizontalmoveInput < 0)
        {
         transform.localScale = new Vector3(-5, 5, 1);
        }

        HorizontalmoveInput = 0f;
    }

    private void HandleJump()
    {

        // Jump when X is pressed and player is grounded
        if (Input.GetKeyDown(KeyCode.X) && isGrounded)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
        }
        
        if (Input.GetKeyUp(KeyCode.X) && rb2d.linearVelocity.y > 0)
        {
            // Cut the upward velocity in half (tweak as needed)
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, rb2d.linearVelocity.y * 0.5f);
        }
    }

    private void checkIfGrounded()
    {
        // Check if player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("jumpVelocity", rb2d.linearVelocityY);
    }

    private void CheckAttack()
    {  
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1"))
                {
                    comboQueued = true;
                }
                else if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    // Start first attack if not attacking
                    animator.SetTrigger("Attack");
                    comboQueued = false;
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
    
    public void Death()
    {
        //player dies and resets the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
