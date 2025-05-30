using Mush;
using Unity.VisualScripting;
using UnityEngine;

public class MushBoss : MonoBehaviour
{
    public float jumpForce = 10f;
    public float jumpInterval = 5f;

    private Rigidbody2D rb;
    [SerializeField] private float timeScinceOnGround;
    [SerializeField] private bool isGrounded;
    private MushOrb[] orbs;

    void Start()
    {
        isGrounded = true;
        timeScinceOnGround = Time.time;
        rb = GetComponent<Rigidbody2D>();

        orbs = transform.parent.GetComponentsInChildren<MushOrb>();
        foreach (MushOrb orb in orbs)
        {
            orb.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Time.time - timeScinceOnGround >= jumpInterval && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded)
            {
                timeScinceOnGround = Time.time;
                MushOrb.LaunchAll(orbs);
            }
            isGrounded = true;
        }
    }
}