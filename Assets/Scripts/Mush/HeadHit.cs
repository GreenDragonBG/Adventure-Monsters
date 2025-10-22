using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mush
{
    public class HeadHit : MonoBehaviour
    {
        [SerializeField] private Animator animator; 

        private bool inRange = false;
        private bool attackScheduled = false;

        public bool canAttack = false;
        private bool isAttacking = false;

        private bool isLeft = false;

        [Header("Close Attack")]
        [SerializeField] private float hitDelay = 1f;
        [SerializeField] private float returnDelay = 0.12f;
        [SerializeField] private float attackCooldown = 4f;
        private float timeEnteredInRange = -Mathf.Infinity;
        private int timesHit = 0;
        
        public float lastAttackTime = -Mathf.Infinity;

        [Header("Shock Wave Attack")]
        [SerializeField] private float firstWaveTime = 6f;
        [SerializeField] private float consecutiveWaveTime = 3f;
        private bool firstWave = true;
        private float currentWaveTimer;
        private ParticleSystem[] wave;
        private ParticleSystem waveLeft;
        private ParticleSystem waveRight;

        private CameraShake camShake;

        private Collider2D playerCollider;

        private void Start()
        {
            wave = transform.GetComponentsInChildren<ParticleSystem>();
            if (wave != null && wave.Length >= 2)
            {
                waveLeft = wave[0];
                waveRight = wave[1];
            }
            calculateWavesDelay();

            camShake = Camera.main?.GetComponent<CameraShake>();
            playerCollider = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (isAttacking) return; // block repeated attacks

            // Wave attack
            if (canAttack && Time.time - lastAttackTime >= (firstWave ? firstWaveTime : currentWaveTimer))
            {
                StartCoroutine(WaveAttack());
            }
            // Tri-hit combo
            else if (inRange && timesHit < 3 && Time.time - timeEnteredInRange >= attackCooldown)
            {
                StartCoroutine(TriHitAttack());
            }
            // Single hit if player stays in after Tri-hit combo
            else if (attackScheduled && Time.time - lastAttackTime >= (attackCooldown / 2) && timesHit > 2)
            {
                StartCoroutine(SingleCapHit());
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                inRange = true;
                attackScheduled = true;
                timeEnteredInRange = Time.time;
                firstWave = true;
                timesHit = 0;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                inRange = false;
                timesHit = 0;
                attackScheduled = false;
            }
        }
        
        //sets the animator and script values on wether the attack is left or right
        private void whereToAttack(Vector3 other, bool isOpposite)
        {
            if (other.x < transform.position.x)
            {
                animator.SetBool("hitIsLeft", !isOpposite);
                isLeft = !isOpposite;
            }
            else
            {
                animator.SetBool("hitIsLeft", isOpposite);
                isLeft = isOpposite;
            }
        }
        //a variation of 'whereToAttack' so you dont need to right the 'isOpposite' boolean
        private void whereToAttack(Vector3 other)
        {
            whereToAttack(other, false);
        }
        

        // Wave attack
        private System.Collections.IEnumerator WaveAttack()
        {
            isAttacking = true;

            firstWave = false;
            whereToAttack(playerCollider.transform.position);
            animator.SetTrigger("Hit");

            if (camShake != null)
                StartCoroutine(camShake.Shake(0.6f, 0.2f));

            if (playerCollider != null)
            {
                if (playerCollider.transform.position.x < transform.position.x)
                    waveLeft.Play();
                else
                    waveRight.Play();
            }

            lastAttackTime = Time.time;
            calculateWavesDelay();

            yield return new WaitForSeconds(hitDelay);
            isAttacking = false;
        }
        //randomly deseides what is the time between evry shockwave
        private void calculateWavesDelay()
        {
            currentWaveTimer = Random.Range(consecutiveWaveTime, consecutiveWaveTime * 2f);
        }

        // Tri-hit combo
        private System.Collections.IEnumerator TriHitAttack()
        {
            isAttacking = true;

            for (int i = 0; i < 3; i++)
            {
                bool opposite = (i == 1); //makes the middle hit opposite based on the boolean answer of 'i == 1'
                CapHit(playerCollider.transform.position, opposite); //uses the close range attack
                yield return new WaitForSeconds(hitDelay); //waits the delay
            }

            isAttacking = false;
            attackScheduled = inRange; //schedules an attack based on if the player is still in range
        }

        // Single hit after combo
        private System.Collections.IEnumerator SingleCapHit()
        {
            isAttacking = true;
            CapHit(playerCollider.transform.position); //uses the close range attack
            yield return new WaitForSeconds(hitDelay); //waits the delay
            isAttacking = false;
        }
    
        //Close Range Attack
        private void CapHit(Vector3 other, bool opposite)
        {
            timesHit++;
            whereToAttack(other, opposite);
            animator.SetTrigger("Hit");
            lastAttackTime = Time.time;
            
            //Add the shacking camera effect
            if (camShake != null)
                StartCoroutine(camShake.Shake(0.6f, 0.2f));
            
            //Deals damage if the player is on the side of the the hit and in range
            if (playerCollider != null && inRange)
            {
                if ((isLeft && playerCollider.transform.position.x < transform.position.x) ||
                    (!isLeft && playerCollider.transform.position.x > transform.position.x))
                {
                    DoDamage.DealDamage();
                }
            }
        }
        
        //a variation of 'whereToAttack' so you dont need to right the 'opposite' boolean
        private void CapHit(Vector3 other)
        {
            CapHit(other, false);
        }
    }
}
