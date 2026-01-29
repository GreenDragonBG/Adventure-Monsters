using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Ale : MonoBehaviour
{
    [Header("What Ability it Unlocks")] 
    [SerializeField]private bool unlockDash;
    
    [Header("Movement Settings")]
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.25f;
    [SerializeField] private float rotationSpeed = 50f;
    
    [Header("Detection Settings")]
    [SerializeField] private float rayDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("References")]
    [SerializeField] private Light2D light2D;
    private Vector3 startPosition;
    private Vector3 baseScale;
    private bool isHovering;
    
    [Header("Instructions")]
    private InstructionsDisplay instructions;
    
    [Header("SelfDestruct")]
    private float timeStartedDestruct = 0f;
    private float inBetweenDestruct = 0f;
    private float endGrowth;
    private Light2D innerLight;

    
    void Start()
    {
        instructions = FindAnyObjectByType<InstructionsDisplay>(FindObjectsInactive.Include);
        if (light2D != null)
        {
            baseScale = light2D.transform.localScale;
        }
    }

    void Update()
    {
        if (timeStartedDestruct > 0f)
        {
            DestroySelf();
        }
        else if (!isHovering)
        {
            HandleFalling();
        }
        else
        {
            HandleHovering();
        }

        if (light2D)
        {
            light2D.transform.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
        }
    }

    private void HandleFalling()
    {
        // 1. Move Down
        transform.Translate(Vector3.down * (fallSpeed * Time.deltaTime));

        // 2. Raycast Check
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, groundLayer);
        
        // Debug line to see the ray in the Scene view
        Debug.DrawRay(transform.position, Vector2.down * rayDistance, Color.red);

        if (hit.collider)
        {
            // Set the "anchor" point for the hover animation to the current spot
            startPosition = transform.position;
            isHovering = true;
        }
    }

    private void HandleHovering()
    {
        float sinValue = Mathf.Sin(Time.time * floatSpeed);
        float newY = startPosition.y + (sinValue * floatHeight);
        
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (light2D != null)
        {
            float sizeMultiplier = Mathf.Lerp(0.5f, 1.0f, (sinValue + 1f) / 2f);
            light2D.transform.localScale = baseScale * sizeMultiplier;
            endGrowth = sizeMultiplier;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           PlayerController pc = other.GetComponent<PlayerController>();

           if (unlockDash)
           {
               pc.canDash = true;
               SaveSystem.CurrentData.hasUnlockedDash = true;
           }
           
           GetComponent<SpriteRenderer>().enabled = false;
           innerLight = GetComponent<Light2D>();
           timeStartedDestruct = Time.time;
           inBetweenDestruct = timeStartedDestruct;
        }
    }


    private void DestroySelf()
    {
        if (Time.time - timeStartedDestruct > 2f)
        {
            instructions.gameObject.SetActive(true);
            Destroy(gameObject);
        }

        if (Time.time - inBetweenDestruct > 0.01f)
        {
            rotationSpeed += 3;
            endGrowth += 0.1f;
            light2D.transform.localScale = baseScale * endGrowth;
            light2D.intensity -= 0.02f;
            innerLight.intensity -= 0.04f;
            inBetweenDestruct =  Time.time;
        }
    }
}