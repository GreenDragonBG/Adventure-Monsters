using Mush;
using Unity.VisualScripting;
using UnityEngine;

public class MushBoss : MonoBehaviour
{
    [SerializeField] public float jumpForce;
    [SerializeField]public float jumpInterval;

    private Rigidbody2D rb;
    private float timeScinceOnGround;
    private bool isGrounded;
    private MushOrb[] orbs;
    private MushSpores spores;

    void Start()
    {
        //finds the spores and orbs and make the orbs inactive
        spores = transform.parent.GetComponentInChildren<MushSpores>();
        orbs = transform.parent.GetComponentsInChildren<MushOrb>();
        foreach (MushOrb orb in orbs)
        {
            orb.gameObject.SetActive(false);
        }
        
        //sets that the Boss starts on the ground and find Rigidbody2D
        isGrounded = true;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //checks if its time to jump and if it's he launches
        //sets isGrounded to false
        if (((Time.time - timeScinceOnGround) >= jumpInterval) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //if it touches the ground after being in the air it lauches the orbs and calculates if to send spors
        //sets isGrounded to true
        if (other.gameObject.CompareTag("Ground") && !isGrounded)
        {
            timeScinceOnGround = Time.time;
            MushOrb.LaunchAll(orbs);
            spores.CalculateChance();
            isGrounded = true;
        }
    }
}