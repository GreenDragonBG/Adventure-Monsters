using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("References")]
    private Transform _player;
    private Rigidbody2D _rb;
    private Animator _anim;

    [Header("Interaction Logic")]
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private GameObject DialogBox;
    private bool _boxIsMoving;
	private TextMeshProUGUI _dialogText;
	private Image _dialogIcon;
	[SerializeField] private Sprite icon;
    [SerializeField] private string dialog;
    private string[] _dialogLines;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDistance = 0.6f;
    [SerializeField] private float slowDistance = 1f;
    [SerializeField] private float viewRange = 5f;
    [SerializeField] private float rotationOffset = 0.345f;
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

        if (isInteractable)
        {
            _dialogLines = dialog.Split("\\");
        }
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
        if (!_player) return;

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
            StartCoroutine(Interact(distanceToPlayer));
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
            transform.position += new Vector3(-rotationOffset, 0f, 0f);
        }
        else if (_previousScaleX < 0 && newScaleX > 0)
        {
            // Flipped from Left to Right
            transform.position += new Vector3(rotationOffset, 0f, 0f);
        }

        // Apply the scale and save for the next check
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        _previousScaleX = newScaleX;
    }

    private IEnumerator Interact(float distanceToPlayer)
    {
        if(!isInteractable) yield return null;

        if (distanceToPlayer <= stopDistance && Input.GetKeyDown(KeyCode.Space))
        {
            _boxIsMoving = true;
            StartCoroutine(MoveDialogBoxUp());
            while (_boxIsMoving)
            {
                yield return null;
            }
            foreach (string dialogLine in _dialogLines)
            {
                _dialogText.text = dialogLine;
                yield return new WaitForSeconds((float)dialogLine.Length /2);
            }
            _dialogText.text = string.Empty;
            _dialogIcon.sprite = null;
            DialogBox.SetActive(false);
        }
    }

    private IEnumerator MoveDialogBoxUp()
    {
        DialogBox.SetActive(true);
        _dialogText = DialogBox.GetComponentInChildren<TextMeshProUGUI>();
        _dialogIcon = DialogBox.GetComponentsInChildren<Image>()[1];
        _dialogIcon.sprite = icon;

        float tempY = DialogBox.transform.position.y;

        while (DialogBox.transform.position.y < 0)
        {
            tempY += 0.2f;
            DialogBox.transform.position = new Vector2(DialogBox.transform.position.x,tempY);
            
            yield return new WaitForSeconds(0.0001f);
        }

        _boxIsMoving = false;
    }
}