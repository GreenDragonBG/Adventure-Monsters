using System;
using UnityEngine;

public class MushSlug : MonoBehaviour
{
    private float speed = 1f;
    public int direction = -1;
    
    // Layer mask for ground
    private int groundLayerMask;

    private void Awake()
    {
        // Only include the "Ground" layer
        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
    }
    void Update()
    {
        // Position where DOWN ray ends
        Vector2 downRayOrigin = transform.position + transform.TransformDirection(new Vector2(0, -0.2f));

        // LOCAL directions
        Vector2 localDown  = transform.TransformDirection(Vector2.down);
        Vector2 localRight = transform.TransformDirection(Vector2.right);
        Vector2 localLeft  = transform.TransformDirection(Vector2.left);

        // --- Ground Check ---
        RaycastHit2D groundCheck = Physics2D.Raycast(downRayOrigin, localDown, 0.3f, groundLayerMask);

        if (groundCheck.collider != null)
        {
            // Walk forward on the current surface (local space)
            transform.Translate(Vector3.right * (direction * speed * Time.deltaTime), Space.Self);
            return;
        }

        // The DOWN ray failed → check walls using the *end of down ray* as origin
        Vector2 sideRayOrigin = downRayOrigin + localDown * 0.5f;     // visualize origin

        // Use scale to make offsets relative
        float scaleX = transform.localScale.x;
        float scaleY = transform.localScale.y;

        Vector3 offsetDown = localDown * (0.5f * scaleY);     // distance down relative to height
        Vector3 offsetSide = localRight * (0.4f * scaleX);   // distance sideways relative to width

        // Moving LEFT
        if (direction == -1)
        {
            RaycastHit2D leftCheck = Physics2D.Raycast(sideRayOrigin, localRight, 0.2f , groundLayerMask);

            if (leftCheck.collider != null)
            {
                transform.Rotate(0, 0, 90);
                // Apply scale-relative offset
                transform.position += offsetDown - offsetSide;
            }
        }
        // Moving RIGHT
        else if (direction == 1)
        {
            RaycastHit2D rightCheck = Physics2D.Raycast(sideRayOrigin, localLeft, 0.2f , groundLayerMask);

            if (rightCheck.collider != null)
            {
                transform.Rotate(0, 0, -90);
                transform.position += offsetDown - offsetSide;
            }
        }
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            transform.Rotate(0, 0,  direction == -1 ? -90f : 90f);
        }
    }
}
