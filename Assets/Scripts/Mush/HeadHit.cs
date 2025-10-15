using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Mush
{
    public class HeadHit : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        private float timeEnteredRange = 0f;
        private bool canHit= false;
        private bool returnToDefault = false;
        private bool inRange = false;
        [SerializeField] private float hitDelay = 2f;
        [SerializeField] private float returnDelay = 1f;
        
        private Rigidbody2D playerRB;
        private Camera cam;

        private void Start()
        {
            cam= Camera.main.GetComponent<Camera>();
        }

        private void Update()
        {
            if (canHit && Time.time-timeEnteredRange >= hitDelay)
            {
                animator.SetTrigger("Hit");
                canHit = false;
                returnToDefault = true;
                if (inRange)
                {
                    DoDamage.DealDamage();
                    playerRB.AddForce(Vector2.left * 10f, ForceMode2D.Impulse);
                }

                StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.6f, 0.2f));
            }

            if (returnToDefault && Time.time-timeEnteredRange >= (hitDelay+returnDelay))
            {
                returnToDefault=false;
            }
        }
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            //if player enters 5 sec after the last attack sets the enter time
            if (other.CompareTag("Player") && Time.time-timeEnteredRange>= (hitDelay+returnDelay+3f))
            {
                inRange = true;
                timeEnteredRange = Time.time;
                canHit = true;
                playerRB = other.GetComponent<Rigidbody2D>();
                
                //checks if the player enters from the right or left
                if (other.transform.position.x < transform.position.x)
                {
                    animator.SetBool("hitIsLeft", true);
                }
                else
                {
                    animator.SetBool("hitIsLeft", false);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                inRange = false;
                playerRB = null;
            }
        }


    }
}