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
        private bool returnToDefault = false;

        public bool canAttack = false;
        
        [Header("Normal Close Attack")]
        [SerializeField] private float hitDelay = 1f;
        [SerializeField] private float returnDelay = 0.12f;
        [SerializeField] private float attackCooldown = 4f;
        private float timeEnteredInRange =  -Mathf.Infinity;
        
        private float nextAttackTime = 0f;
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
            camShake = Camera.main?.GetComponent<CameraShake>();
            wave = transform.GetComponentsInChildren<ParticleSystem>();
            waveLeft = wave[0];
            waveRight = wave[1];
            calculateWavesDelay();
            
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        }

        private void Update()
        {

            if (canAttack && Time.time - lastAttackTime >= (firstWave ? firstWaveTime : currentWaveTimer))
            {
                WaveAttack();
                
                if (inRange)
                {
                    DoDamage.DealDamage();
                }
            } 
            
            if (attackScheduled && Time.time-timeEnteredInRange>=attackCooldown && Time.time >= nextAttackTime)
            {
                whereToAttack(playerCollider);
                animator.SetTrigger("Hit");
                returnToDefault = true;
                lastAttackTime= Time.time;

                if (camShake != null)
                    StartCoroutine(camShake.Shake(0.6f, 0.2f));

                if (inRange)
                {
                    DoDamage.DealDamage();
                }


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
                inRange = true;
                attackScheduled = true;
                
                nextAttackTime = Time.time + hitDelay;
                firstWave = true;
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

        private void calculateWavesDelay()
        {
            currentWaveTimer = Random.Range(consecutiveWaveTime, consecutiveWaveTime* 2f);
        }

        private void WaveAttack()
        {
            firstWave = false;
            whereToAttack(playerCollider);
            animator.SetTrigger("Hit");
            returnToDefault = true;
                
            if (camShake != null)
                StartCoroutine(camShake.Shake(0.6f, 0.2f));
                
            // play correct wave direction
            if (playerCollider != null) 
            {
                if (playerCollider.transform.position.x < transform.position.x)
                    waveLeft.Play();
                else
                    waveRight.Play();
            }

            lastAttackTime = Time.time;
            calculateWavesDelay();
        }
    }
}
