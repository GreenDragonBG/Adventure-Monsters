using System;
using UnityEngine;

namespace Mush
{
    public class HeadHit : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private bool inRange = false;
        private bool attackScheduled = false;
        private bool returnToDefault = false;

        [SerializeField] private float hitDelay = 0.5f;
        [SerializeField] private float returnDelay = 0.12f;
        [SerializeField] private float attackCooldown = 2f;

        private float nextAttackTime = 0f;
        private float lastAttackTime = -Mathf.Infinity;

        private CameraShake camShake;

        private Collider2D playerCollider;
        private void Start()
        {
            camShake = Camera.main?.GetComponent<CameraShake>();
        }

        private void Update()
        {
            if (attackScheduled && Time.time-lastAttackTime>=attackCooldown && Time.time >= nextAttackTime)
            {
                whereToAttack(playerCollider);
                animator.SetTrigger("Hit");
                returnToDefault = true;
                lastAttackTime= Time.time;

                if (camShake != null)
                    StartCoroutine(camShake.Shake(0.6f, 0.2f));

                if (inRange)
                    DoDamage.DealDamage();

                
                nextAttackTime = Time.time + hitDelay;

                // Schedule next attack only if player is still in range
                attackScheduled = inRange;
            }

            if (returnToDefault && Time.time >= nextAttackTime - returnDelay)
            {
                returnToDefault = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerCollider= other;
                inRange = true;
                attackScheduled = true;

               

                nextAttackTime = Time.time + hitDelay;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                inRange = false;
        }

        private void whereToAttack(Collider2D other)
        {
            if (other.transform.position.x < transform.position.x)
                animator.SetBool("hitIsLeft", true);
            else
                animator.SetBool("hitIsLeft", false);
        }
    }
}
