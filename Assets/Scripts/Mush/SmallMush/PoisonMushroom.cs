using UnityEngine;
public class PoisonMushroom : MushroomController
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time - lastAttackTime >= attackDelay)
        {
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


    public void PoisonAttack()
    {
        //playerRb.linearVelocity = Vector2.zero;
        //playerController.canMove = false;

        DoDamage.DealDamage();
        playerRb.gameObject.GetComponent<Animator>().SetTrigger("Damage");
        //playerRb.AddForce(new Vector2(7 * (playerIsBehind ? 1f : -1f) * (facingRight ? -1f : 1f), 8), ForceMode2D.Impulse);

        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
