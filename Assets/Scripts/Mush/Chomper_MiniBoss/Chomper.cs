using UnityEngine;
using System.Collections;

public class Chomper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDistance = 0.4f;
    [SerializeField] private float slowDistance = 1f; // start slowing when close
    [SerializeField] private float viewRange = 5f; // how far the enemy can see

    [Header("Roaming")]
    [SerializeField] private float roamTimeMin = 1f;
    [SerializeField] private float roamTimeMax = 3f;

    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkDistance = 0.15f;

    private bool groundAhead;
    private bool wallAhead;
    private bool isRoaming = false;
    private int roamDirection = 1;
    private float previousScaleX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        previousScaleX = transform.localScale.x;
        StartCoroutine(RoamRoutine());
    }

    private void FixedUpdate()
    {
        if (!player) return;

        UpdateChecks();
        Move();
    }

    private void UpdateChecks()
    {
        groundAhead = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            checkDistance,
            groundLayer
        );

        wallAhead = Physics2D.Raycast(
            wallCheck.position,
            transform.right,
            checkDistance,
            groundLayer
        );
    }

    private void Move()
    {
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        // Player in view → chase
        if (distanceToPlayer <= viewRange)
        {
            isRoaming = false;

            float direction = Mathf.Sign(player.position.x - transform.position.x);

            FlipWithOffset(direction);

            if (distanceToPlayer <= stopDistance || wallAhead || !groundAhead)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                anim.SetBool("isWalking", false);
                return;
            }

            float moveSpeed = speed;
            if (distanceToPlayer < slowDistance)
            {
                moveSpeed *= distanceToPlayer / slowDistance;
            }

            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            anim.SetBool("isWalking", true);
            return;
        }

        // Player out of view → roam
        isRoaming = true;
        RoamMove();
    }

    private void RoamMove()
    {
        if (wallAhead || !groundAhead)
        {
            roamDirection *= -1; // flip direction
        }

        rb.linearVelocity = new Vector2(roamDirection * speed, rb.linearVelocity.y);
        FlipWithOffset(roamDirection);
        anim.SetBool("isWalking", true);
    }

    private void FlipWithOffset(float direction)
    {
        float newScaleX = -Mathf.Abs(transform.localScale.x) * direction;

        // Check for flip
        if (previousScaleX > 0 && newScaleX < 0)
        {
            transform.position += new Vector3(1.12f, 0f, 0f);
        }
        else if (previousScaleX < 0 && newScaleX > 0)
        {
            transform.position += new Vector3(-1.12f, 0f, 0f);
        }

        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        previousScaleX = newScaleX;
    }

    private IEnumerator RoamRoutine()
    {
        while (true)
        {
            if (isRoaming)
            {
                roamDirection = Random.value < 0.5f ? -1 : 1;
                float roamTime = Random.Range(roamTimeMin, roamTimeMax);
                yield return new WaitForSeconds(roamTime);
            }
            else
            {
                yield return null;
            }
        }
    }
}
