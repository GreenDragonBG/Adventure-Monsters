using System;
using System.Collections;
using UnityEngine;

public class MushSlug : MonoBehaviour
{
    private int health = 60;
    private Animator anim;
    private bool isDeath;
    private const float Speed = 1f;
    private float direction = -1;
    
    private CapsuleCollider2D deathCollider;
    
    // Layer mask for ground
    private int groundLayerMask;

    private void Awake()
    {
        // Only include the "Ground" layer
        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Start()
    {
        direction= transform.localScale.x>0 ? -1 : 1;
        anim = GetComponent<Animator>();
        deathCollider = GetComponent<CapsuleCollider2D>();
        deathCollider.enabled = false;
    }

    private void Update()
    {
        if (!isDeath && health<=0)
        {
            StartCoroutine(Death());
        }
    }

    void FixedUpdate()
    {
        if(isDeath) return; //Doesnt continue if the slug is death
        
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
            transform.Translate(Vector3.right * (direction * Speed * Time.deltaTime), Space.Self);
            return;
        }

        // The DOWN ray failed → check walls using the *end of down ray* as origin
        Vector2 sideRayOrigin = downRayOrigin + localDown * 0.5f;     // visualize origin

        // Use scale to make offsets relative
        float scaleX = transform.localScale.x;
        float scaleY = transform.localScale.y;

        Vector3 offsetDown = localDown * (0.5f * scaleY);     // distance down relative to height
        Vector3 offsetSide = localRight * (0.4f * scaleX);   // distance sideways relative to width

        
        RaycastHit2D sideCheck = Physics2D.Raycast(sideRayOrigin, direction == -1 ? localRight: localLeft, 0.2f , groundLayerMask);

        if (sideCheck.collider != null)
        {
            transform.Rotate(0, 0, direction == -1 ? 90f : -90f);
            // Apply scale-relative offset
            transform.position += offsetDown - offsetSide;
        }
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isDeath) return; //Doesnt continue if the slug is death

        if (other.CompareTag("Ground"))
        {
            transform.Rotate(0, 0,  direction == -1 ? -90f : 90f);
        }
        else if (other.CompareTag("Attack"))
        {
            health -= other.transform.parent.GetComponent<PlayerController>().attackDamage;
            anim.SetTrigger("damaged");
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<Animator>().SetTrigger("Damage");
            DoDamage.DealDamage();
        }
    }

    private IEnumerator Death()
    {
        isDeath = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        deathCollider.enabled = true;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        
    }
}
