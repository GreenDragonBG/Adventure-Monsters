using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private float checkDistance = 1.5f; // Distance to check for ground
    private GameObject enemy;
    private Rigidbody2D rb;
    public LayerMask groundLayer; // Assign this in the Inspector

    private void Start()
    {
        enemy = gameObject;
        rb = enemy.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on enemy!");
        }
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, groundLayer);
        
        Debug.DrawRay(transform.position, Vector2.down * checkDistance, Color.yellow); // Shows the ray in Scene view

        if (hit.collider != null)
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name); // Shows what the ray hits
            
            if (hit.collider.CompareTag("Ground"))  // Correct way to check tag
            {
                Debug.Log("Ground detected!");
                rb.AddForce(Vector2.up * 2, ForceMode2D.Impulse);
            }
        }
    }
}