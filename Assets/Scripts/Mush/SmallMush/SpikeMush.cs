using UnityEngine;

public class SpikeMush : MushroomController
{
    private bool playerIsBehind = false;
    private bool playerInRange = false;

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);

        if (other.collider.CompareTag("Player") && Time.time - lastAttackTime >= attackDelay)
        {
            playerInRange = true;
            if (!isGrounded)
            {
                hasToWait = true;
            }
            else
            {
                SetOfAttack();
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    protected override void SetOfAttack()
    {
        base.SetOfAttack();

        // Determine if the player is behind the mushroom
        if (playerTransform.position.x - 0.02f < transform.position.x)
        {
            playerIsBehind = facingRight;
        }
        else
        {
            playerIsBehind = !facingRight;
        }
    }

    public void SpikeAttack()
    {
        if (!playerInRange)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            return;
        }

        playerRb.linearVelocity = Vector2.zero;
        playerController.canMove = false;

        DoDamage.DealDamage();
        playerRb.gameObject.GetComponent<Animator>().SetTrigger("Damage");
        playerRb.AddForce(new Vector2(7 * (playerIsBehind ? 1f : -1f) * (facingRight ? -1f : 1f), 8), ForceMode2D.Impulse);

        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}