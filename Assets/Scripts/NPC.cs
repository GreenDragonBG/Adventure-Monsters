using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("References")]
    private Transform _player;
    private Rigidbody2D _rb;
    private Animator _anim;

    [Header("Interaction Logic")]
    [SerializeField] private bool isInteractable = true;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDistance = 0.6f;
    [SerializeField] private float slowDistance = 1f;
    [SerializeField] private float viewRange = 5f;
    private float _previousScaleX; // Tracked for offset logic

    [Header("Roaming Settings")]
    [SerializeField] private float roamTimeMin = 1f;
    [SerializeField] private float roamTimeMax = 3f;
    private bool _isRoaming = true;
    private int _roamDirection = 1;

    [Header("Environment Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkDistance = 0.15f;
    private bool _groundAhead;
    private bool _wallAhead;

    // Animation Hash
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        if (_player == null) 
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        _previousScaleX = transform.localScale.x; // Initialize scale tracker
        StartCoroutine(RoamRoutine());
    }

    void FixedUpdate()
    {
        UpdateChecks();
        HandleBehavior();
    }

    private void UpdateChecks()
    {
        _groundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
        
        float lookDir = _isRoaming ? _roamDirection : Mathf.Sign(_rb.linearVelocity.x);
        // Note: Use transform.right * lookDir or simply Vector2.right * lookDir depending on setup
        _wallAhead = Physics2D.Raycast(wallCheck.position, Vector2.right * lookDir, checkDistance, groundLayer);
    }

    private void HandleBehavior()
    {
        if (_player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        if (isInteractable && distanceToPlayer <= viewRange)
        {
            _isRoaming = false;
            MoveTowardsPlayer(distanceToPlayer);
        }
        else
        {
            _isRoaming = true;
        }
    }

    private void MoveTowardsPlayer(float distanceToPlayer)
    {
        float direction = Mathf.Sign(_player.position.x - transform.position.x);
        FlipWithOffset(direction);

        if (distanceToPlayer <= stopDistance || _wallAhead || !_groundAhead)
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            _anim.SetBool(IsWalking, false);
            return;
        }

        float moveSpeed = speed;
        if (distanceToPlayer < slowDistance)
            moveSpeed *= (distanceToPlayer / slowDistance);

        _rb.linearVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);
        _anim.SetBool(IsWalking, true);
    }

    private void RoamMove()
    {
        if (_wallAhead || !_groundAhead)
        {
            _roamDirection *= -1;
        }

        _rb.linearVelocity = new Vector2(_roamDirection * speed, _rb.linearVelocity.y);
        FlipWithOffset(_roamDirection);
        _anim.SetBool(IsWalking, true);
    }

    private IEnumerator RoamRoutine()
    {
        while (true)
        {
            if (_isRoaming)
            {
                _roamDirection = Random.value < 0.5f ? -1 : 1;
                float roamDuration = Random.Range(roamTimeMin, roamTimeMax);
                float elapsed = 0f;

                while (elapsed < roamDuration && _isRoaming)
                {
                    RoamMove();
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                
                _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
                _anim.SetBool(IsWalking, false);
                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            }
            else
            {
                yield return new WaitForSeconds(0.2f); 
            }
        }
    }

    private void FlipWithOffset(float direction)
    {
        if (direction == 0) return;

        // Calculate what the new scale should be based on direction
        // Assuming -Scale means facing left and +Scale means facing right (or vice-versa)
        float newScaleX = Mathf.Abs(transform.localScale.x) * Mathf.Sign(direction);

        // Check if a flip is actually happening
        if (_previousScaleX > 0 && newScaleX < 0)
        {
            // Flipped from Right to Left
            transform.position += new Vector3(-0.345f, 0f, 0f);
        }
        else if (_previousScaleX < 0 && newScaleX > 0)
        {
            // Flipped from Left to Right
            transform.position += new Vector3(0.345f, 0f, 0f);
        }

        // Apply the scale and save for the next check
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        _previousScaleX = newScaleX;
    }
}