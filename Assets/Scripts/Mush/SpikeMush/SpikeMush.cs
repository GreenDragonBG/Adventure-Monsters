using System;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeMush : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;       // Upward force
    public float forwardForce = 3f;    // Horizontal force

    [Header("Direction")]
    public bool facingRight = true;    // Change if your sprite faces left
    public float lastTimeHitAWall = -Mathf.Infinity;
    
    private Rigidbody2D rb;
    public bool isGrounded = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private float attackDelay = 1f;
    private float lastAttackTime = -Mathf.Infinity;
    private bool hasToWait = false;
    
    [Header("Player variables")]
    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerController playerController;
    private bool playerIsBehind = false;
    private bool playerInRange = false;

    private void Update()
    {
        if (!hasToWait && !playerController.canMove && animator.GetCurrentAnimatorStateInfo(0).IsName("JumpMove"))
        {
            playerController.canMove = true;
        }

        if (isGrounded && hasToWait)
        {
            hasToWait = false;
            SetOfAttack();
        }
    }

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerRb = playerTransform.gameObject.GetComponent<Rigidbody2D>();
        playerController = playerTransform.gameObject.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Call this from the animation event
    public void Launch()
    {
        if (!isGrounded) return;

        // Reset velocity for consistent jump
        rb.linearVelocity = Vector2.zero;

        // Calculate direction
        float direction = facingRight ? 1f : -1f;

        // Apply jump impulse
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(forwardForce * direction, jumpForce), ForceMode2D.Impulse);

        isGrounded = false;
    }

    // Detect landing (assuming the ground has tag "Ground")
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player") && Time.time-lastAttackTime >= attackDelay)
        {
            playerInRange = true;
            if (!isGrounded)
            {
                hasToWait = true;
            }
            else
            {
                SetOfAttack();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerInRange = false;
    }

    // Optional: Flip enemy direction
    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void SetOfAttack()
    {
        rb.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("Attack");
            
        if (playerTransform.position.x-0.02 < transform.position.x)
        {
            playerIsBehind = facingRight;
        }
        else
        {
            playerIsBehind = !facingRight;
        }
    }
    
    public void SpikeAttack()
    {
        
        if (!playerInRange)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            return;
        }

        playerRb.linearVelocity = Vector2.zero;
        playerController.canMove = false;
        DoDamage.DealDamage();
        playerRb.gameObject.GetComponent<Animator>().SetTrigger("Damage");
        playerRb.AddForce(new Vector2((7 * (playerIsBehind ? 1f : -1f) * (facingRight ? -1f : 1f)), 8), ForceMode2D.Impulse);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
