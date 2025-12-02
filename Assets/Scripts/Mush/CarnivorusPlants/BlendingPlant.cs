using System.Collections;
using UnityEngine;

public class BlendingPlant : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private bool canDoDamage = true;
    private bool playerInside = false;
    private Coroutine attackRoutine;

    [Header("Animation")]
    private Animator animator;
    private Animator playerAnimator;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerAnimator = other.GetComponent<Animator>();

            if (attackRoutine == null)
                attackRoutine = StartCoroutine(RepeatAttack());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
        }
    }

    private IEnumerator RepeatAttack()
    {
        yield return new WaitForSeconds(2f);

        while (playerInside)
        {
            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(4f);
        }
    }

    private void DealTheDamage()
    {
        if (canDoDamage)
        {
            playerAnimator.SetTrigger("Damage");
            DoDamage.DealDamage();
        }
    }
}