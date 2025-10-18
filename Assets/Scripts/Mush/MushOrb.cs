using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mush
{
    public class MushOrb :MonoBehaviour
    {
        [SerializeField] public float launcForceX;
        [SerializeField] public float launcForceY;
        private Rigidbody2D rb;
        private Vector3 startPos;
        private static bool blastIsTriggered = false;

        private void Start()
        {
            //finds Rigidbody2D and it's starting position
            rb = GetComponent<Rigidbody2D>();
            startPos = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {  
            //checks if it hits the deadzone
            //if it does it get's teleported to its starting position and deactivated
            if (other.gameObject.name == "OrbDeadZone")
            {
                transform.position = startPos;
                blastIsTriggered = false;
                gameObject.SetActive(false);
            }
            
            if (other.gameObject.name == "OrbBlastZone")
            {
                if (blastIsTriggered)
                {
                    return;
                }
                
                blastIsTriggered = true;
                int trigeredBlasts = 0;
                
                foreach (ParticleSystem particle in other.GetComponentsInChildren<ParticleSystem>())
                {
                    // firts blast has 1 out of 4 chance
                    int roll = Random.Range(0, 4);
                    if (roll == 0)
                    {
                        trigeredBlasts++;
                        particle.Play();
                    }
                }
            }

            if (other.CompareTag("Player"))
            {
                DoDamage.DealDamage();
                other.GetComponent<Animator>().SetTrigger("Damage");
            }
        }

        public void Launch()
        {
            rb.linearVelocity = new Vector2(launcForceX,launcForceY);
        }

    }
}