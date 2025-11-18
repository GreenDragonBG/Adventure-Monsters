using UnityEngine;

public class SpikeMushroom : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;        // Upward force
    public float forwardForce = 3f;     // Horizontal force

    [Header("Direction")]
    public bool facingRight = true;     // Change if your sprite faces left
    public float lastTimeHitAWall = -Mathf.Infinity;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private float attackDelay = 1f;
    private float lastAttackTime = -Mathf.Infinity;

    [Header("Player variables")]
    private Transform playerTransform;
    private Rigidbody2D playerRb;
    private PlayerController playerController;

    private Rigidbody2D rb;
    private bool isGrounded = true;

    // Spike-specific
    private bool playerIsBehind = false;
    private bool playerInRange = false;

    protected virtual void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerRb = playerTransform.gameObject.GetComponent<Rigidbody2D>();
        playerController = playerTransform.gameObject.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        // Re-enable player movement when attack animation ends
        if (!playerController.canMove && animator.GetCurrentAnimatorStateInfo(0).IsName("JumpMove"))
        {
            playerController.canMove = true;
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
        // Ground check
        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            return;
        }

        // Player hit
        if (other.collider.CompareTag("Player") && Time.time - lastAttackTime >= attackDelay)
        {
            playerInRange = true;

            SetOfAttack();
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
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
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        animator.SetTrigger("Attack");

        // Determine if player is behind
        if (playerTransform.position.x - 0.02f < transform.position.x)
        {
            playerIsBehind = facingRight;
        }
        else
        {
            playerIsBehind = !facingRight;
        }
    }

    // Called from animation event
    public void SpikeAttack()
    {
        if (!playerInRange)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }

        playerRb.linearVelocity = Vector2.zero;
        playerController.canMove = false;

        DoDamage.DealDamage();
        playerRb.gameObject.GetComponent<Animator>().SetTrigger("Damage");

        playerRb.AddForce(new Vector2(7 * (playerIsBehind ? 1f : -1f) * (facingRight ? -1f : 1f), 8), ForceMode2D.Impulse);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
