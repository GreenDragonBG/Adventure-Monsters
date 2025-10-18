using UnityEngine;

public class MushroomController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;       // Upward force
    public float forwardForce = 3f;    // Horizontal force

    [Header("Direction")]
    public bool facingRight = true;    // Change if your sprite faces left
    public float lastTimeHitAWall = -Mathf.Infinity;

    protected Rigidbody2D rb;
    protected bool isGrounded = true;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    protected float attackDelay = 1f;
    protected float lastAttackTime = -Mathf.Infinity;
    protected bool hasToWait = false;

    [Header("Player variables")]
    protected Transform playerTransform;
    protected Rigidbody2D playerRb;
    protected PlayerController playerController;

    protected virtual void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerRb = playerTransform.gameObject.GetComponent<Rigidbody2D>();
        playerController = playerTransform.gameObject.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
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

    // Launch jump — called from animation event
    public virtual void Launch()
    {
        if (!isGrounded) return;
        
        
        float direction = facingRight ? 1f : -1f;
        rb.linearVelocity = Vector2.zero;
        transform.position += Vector3.up * 0.03f;
        rb.AddForce(new Vector2(forwardForce * direction, jumpForce), ForceMode2D.Impulse);

        isGrounded = false;
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Flip the facing direction
    public virtual void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected virtual void SetOfAttack()
    {
        rb.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("Attack");
    }
}
