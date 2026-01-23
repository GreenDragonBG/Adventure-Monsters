using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Ale : MonoBehaviour
{
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
    private bool isHovering = false;

    void Start()
    {
        if (light2D != null)
        {
            baseScale = light2D.transform.localScale;
        }
    }

    void Update()
    {
        if (!isHovering)
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
        }
    }
}