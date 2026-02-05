using System;
using System.Collections;
using Saves;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Main Settings")] 
    [SerializeField] private EscapeMenu escapeMenu;
    private Rigidbody2D _rb2d;
    public int playerHealth = 90;
    private Animator _animator;
    private Camera _cam;
    private CameraController _camController;
    private CameraShake _camShake;
    public bool canMove = true; 
    public float speed;
    public static bool ShouldTeleportToSave = true;
   


    [Header("Attack")]
    public int attackDamage = 30;
    private Collider2D _attackRange;
    
    [Header("Horizontal Movement")]
    public float horizontalMoveInput = 0f;
    
    
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public bool isGrounded;
    
    [Header("Long Fall")]
    [SerializeField] private float longFallThreshold = 6f;
    [SerializeField] private Animator longFallSmokeAnimator;
    private bool _wasFalling = false;
    private bool _hasEnteredThreshold = false;
    private float _fallStartY = 0f;


    [Header("Abilities")] 
    public bool canDash = true;
    [SerializeField]private CooldownUI dashCooldownUI;
    [SerializeField]private GameObject dashPrefab;

    private void Awake()
    {
        
        SaveSystem.LoadFromFile();
        if (ShouldTeleportToSave && !SaveSystem.CurrentData.isNewGame && SceneManager.GetActiveScene().name == SaveSystem.CurrentData.lastScene)
        {
                StartCoroutine(TeleportToSave());
                canDash = SaveSystem.CurrentData.hasUnlockedDash;
        }
        
        OptionsSave.LoadOptions();
    }
    
    private IEnumerator TeleportToSave()
    {
        // Wait for the physics engine to settle
        yield return new WaitForEndOfFrame();
        transform.position = SaveSystem.CurrentData.playerPos;
        ShouldTeleportToSave = false;
    }
    
    void Start()
    {
        canMove= true;
        _rb2d= GetComponent<Rigidbody2D>();
        _animator= GetComponent<Animator>();
        _cam = Camera.main;
        if (_cam != null)
        {
            _camController = _cam.GetComponent<CameraController>();
            _camShake = _cam.GetComponent<CameraShake>();
        }

        foreach (Collider2D coll in GetComponentsInChildren<Collider2D>())
        {
            if (coll.gameObject.CompareTag("Attack"))
            {
                _attackRange = coll;
                _attackRange.enabled = false;
            }
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt)  && Input.GetKeyDown(KeyCode.D))
        {
            SaveSystem.ClearSave();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !escapeMenu.gameObject.activeSelf)
        {
            Time.timeScale = 0f;
            escapeMenu.gameObject.SetActive(true);
        }

        //Methods that dont need for the player to move
        CheckIfGrounded(); 
        HandleFallingDistance();
        
        if (!canMove) return;
        //Methods that need for the player to be able to move
        CheckAttack();
        HandleJump();
        HandleHorizontalMovement();
        //Abilities
        HandleDash();
    }
    private void HandleHorizontalMovement()
    {

        if (horizontalMoveInput == 0)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalMoveInput = -1f;
            } else if (Input.GetKey(KeyCode.RightArrow))
            {
                horizontalMoveInput = 1f;
            }
        }

        // Apply movement
        _rb2d.linearVelocity = new Vector2(horizontalMoveInput * speed, _rb2d.linearVelocity.y);
        if (_rb2d.linearVelocity.x > 0.1 || _rb2d.linearVelocity.x < -0.1)
        {
            _animator.SetBool("IsRunning", true);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }

        // Flip the character sprite based on direction
        if (horizontalMoveInput > 0) {
         transform.localScale = new Vector3(5, 5, 1);
        }else if (horizontalMoveInput < 0)
        {
         transform.localScale = new Vector3(-5, 5, 1);
        }

        horizontalMoveInput = 0f;
    }

    private void HandleJump()
    {

        // Jump when X is pressed and player is grounded
        if (Input.GetKeyDown(KeyCode.X) && isGrounded)
        {
            _rb2d.linearVelocity = new Vector2(_rb2d.linearVelocity.x, jumpForce);
        }
        
        if (Input.GetKeyUp(KeyCode.X) && _rb2d.linearVelocity.y > 0)
        {
            // Cut the upward velocity in half (tweak as needed)
            _rb2d.linearVelocity = new Vector2(_rb2d.linearVelocity.x, _rb2d.linearVelocity.y * 0.5f);
        }
    }

    private void CheckIfGrounded()
    {
        // Check if player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        _animator.SetBool("IsGrounded", isGrounded);
        _animator.SetFloat("jumpVelocity", _rb2d.linearVelocityY);
    }

    private void CheckAttack()
    {  
            if (Input.GetKeyDown(KeyCode.Z) && !_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            { 
                _animator.SetTrigger("Attack");
            }
    }
    
    
    
    private void OnLongFallLanding()
    {
        _camController.smoothValue = 2f;
        longFallSmokeAnimator.SetTrigger("LandEffect");
        _rb2d.gravityScale = 3f;
        float fallDistance = _fallStartY - transform.position.y;

        // Trigger shake scaled by fall distance
        TriggerScaledCameraShake(fallDistance);
        
        StartCoroutine(MovementImpactAfterLongFall());
    }
    
    private void TriggerScaledCameraShake(float fallDistance)
    {
        // Falling 6 meters = minimum shake
        // Falling 20+ meters = maximum shake
        float t = Mathf.InverseLerp(longFallThreshold, 20f, fallDistance);

        float shakeDuration = Mathf.Lerp(0.4f, 1f, t);   // 0.4 → 1 seconds
        float shakeStrength = Mathf.Lerp(0.05f, 0.4f, t); // 0.05 → 0.4 intensity
        StartCoroutine(_camShake.Shake(shakeDuration, shakeStrength));
    }
    
    private IEnumerator SmoothCameraSmoothness(float targetValue, float duration)
    {
        float startValue = _camController.smoothValue;
        float t = 0f;

        while (t < duration)
        {
            _camController.smoothValue = Mathf.Lerp(startValue, targetValue, t / duration);
            t += Time.deltaTime;
            if (isGrounded)
            {
                _camController.smoothValue = 2f;
                yield break;
            }

            yield return null;
        }

        _camController.smoothValue = targetValue; // ensure exact
    }

    private IEnumerator MovementImpactAfterLongFall()
    {
        _animator.SetTrigger("HeavyLanding");
        horizontalMoveInput = 0;
        _rb2d.linearVelocity= Vector2.zero;
        canMove = false;
        yield return new WaitForSeconds(1.5f);
        canMove = true;
    }
    
    private void OnEnterLongFallThreshold()
    {
        if(_hasEnteredThreshold) return;
        
        StartCoroutine(SmoothCameraSmoothness(6f, 0.08f));
        _rb2d.linearVelocity = new Vector2(_rb2d.linearVelocity.x, -20f);
        _rb2d.gravityScale = 0f;
        
        _hasEnteredThreshold = true;
    }

    private void HandleFallingDistance()
    {
        // Player is falling (velocity < 0) and NOT grounded
        bool isFalling = _rb2d.linearVelocity.y < -0.1f && !isGrounded;
        
        // Start of falling
        if (isFalling && !_wasFalling)
        {
            _wasFalling = true;
            _fallStartY = transform.position.y;
        }

        if (_wasFalling)
        {
            //calc the fallDistance
            float fallDistance = _fallStartY - transform.position.y;
            
            //if has fallen distance over the longFallThreshold 
            if (fallDistance >= longFallThreshold)
            {
                OnEnterLongFallThreshold();
                //if grounded start longFallLanding
                if (!isGrounded) return;    
                OnLongFallLanding();
            }
            
            //If grounded end fall
            if (!isGrounded) return;
            _wasFalling = false;
            _hasEnteredThreshold = false;
        }
    }

    private void HandleDash()
    {
        if (canDash && Input.GetKeyDown(KeyCode.A) && !dashCooldownUI.isActiveAndEnabled)
        {
            Instantiate(dashPrefab,new Vector3(transform.position.x,transform.position.y+0.5f,transform.position.z),
                (transform.localScale.x/Math.Abs(transform.localScale.x))>0 ? new Quaternion(0,0,0,0):new Quaternion(0,180,0,0)); //transform.rotation)
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, 
                (transform.localScale.x/Math.Abs(transform.localScale.x))>0 ? Vector2.right : Vector2.left,
                4f, groundLayer);

            if (!hit2D.collider)
            {
                transform.position = new Vector3(transform.position.x+4*(transform.localScale.x/Math.Abs(transform.localScale.x)), transform.position.y);
            }
            else
            {
                transform.position =hit2D.point;
            }

            dashCooldownUI.gameObject.SetActive(true);
        }
    }


    public void Death()
    {
        ShouldTeleportToSave = true; 
        _animator.enabled = false;
        SaveSystem.ReloadToLastSave();
    }

    //Sets the trigger to be enabled so its able to hit, Its called by the animation
    private void SetAttackRangeOn()
    {
        _attackRange.enabled = true;
    }

    private void SetAttackRangeOff()
    {
        _attackRange.enabled = false;
    }
}
