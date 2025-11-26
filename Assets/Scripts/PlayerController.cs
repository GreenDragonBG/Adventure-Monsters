using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
    [Header("Main Settings")]
    private Rigidbody2D rb2d;
    private Animator animator;
    private Camera cam;
    private CameraController camController;
    private CameraShake camShake;
    public bool canMove = true; 
    private bool comboQueued;
    public float speed;

    public int playerHealth = 90;
    
    
    [Header("Horizontal Movement")]
    public float horizontalmoveInput = 0f;
    
    
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public bool isGrounded;
    
    [Header("Long Fall")]
    [SerializeField] private float longFallThreshold = 6f;
    [SerializeField] private Animator longFallSmokeAnimator;
    private bool wasFalling = false;
    private bool hasEnteredThreshold = false;
    private float fallStartY = 0f;
    

    void Start()
    {
        canMove = true;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
        camController = cam.GetComponent<CameraController>();
        camShake = cam.GetComponent<CameraShake>();
    }

    void Update()
    {
        //Methods that dont need for the player to move
        CheckIfGrounded(); 
        HandleFallingDistance();
        
        if (!canMove) return;
        //Methods that need for the player to be able to move
        CheckAttack();
        HandleJump();
        HandleHorizontalMovement();
    }
    private void HandleHorizontalMovement()
    {

        if (horizontalmoveInput == 0)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalmoveInput = -1f;
            } else if (Input.GetKey(KeyCode.RightArrow))
            {
                horizontalmoveInput = 1f;
            }
        }

        // Apply movement
        rb2d.linearVelocity = new Vector2(horizontalmoveInput * speed, rb2d.linearVelocity.y);
        if (rb2d.linearVelocity.x > 0.1 || rb2d.linearVelocity.x < -0.1)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        // Flip the character sprite based on direction
        if (horizontalmoveInput > 0) {
         transform.localScale = new Vector3(5, 5, 1);
        }else if (horizontalmoveInput < 0)
        {
         transform.localScale = new Vector3(-5, 5, 1);
        }

        horizontalmoveInput = 0f;
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

    private void CheckIfGrounded()
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
    
    
    
    private void OnLongFallLanding()
    {
        camController.smoothValue = 2f;
        longFallSmokeAnimator.SetTrigger("LandEffect");
        rb2d.gravityScale = 3f;
        float fallDistance = fallStartY - transform.position.y;

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
        StartCoroutine(camShake.Shake(shakeDuration, shakeStrength));
    }
    
    private IEnumerator SmoothCameraSmoothness(float targetValue, float duration)
    {
        float startValue = camController.smoothValue;
        float t = 0f;

        while (t < duration)
        {
            camController.smoothValue = Mathf.Lerp(startValue, targetValue, t / duration);
            t += Time.deltaTime;
            if (isGrounded)
            {
                camController.smoothValue = 2f;
                yield break;
            }

            yield return null;
        }

        camController.smoothValue = targetValue; // ensure exact
    }

    private IEnumerator MovementImpactAfterLongFall()
    {
        animator.SetTrigger("HeavyLanding");
        horizontalmoveInput = 0;
        rb2d.linearVelocity= Vector2.zero;
        canMove = false;
        yield return new WaitForSeconds(1.5f);
        canMove = true;
    }
    
    private void OnEnterLongFallThreshold()
    {
        if(hasEnteredThreshold) return;
        
        StartCoroutine(SmoothCameraSmoothness(6f, 0.08f));
        rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, -20f);
        rb2d.gravityScale = 0f;
        
        hasEnteredThreshold = true;
    }

    private void HandleFallingDistance()
    {
        // Player is falling (velocity < 0) and NOT grounded
        bool isFalling = rb2d.linearVelocity.y < -0.1f && !isGrounded;
        
        // Start of falling
        if (isFalling && !wasFalling)
        {
            wasFalling = true;
            fallStartY = transform.position.y;
        }

        if (wasFalling)
        {
            //calc the fallDistance
            float fallDistance = fallStartY - transform.position.y;
            
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
            wasFalling = false;
            hasEnteredThreshold = false;
        }
    }
    
    public void Death()
    {
        //player dies and resets the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
