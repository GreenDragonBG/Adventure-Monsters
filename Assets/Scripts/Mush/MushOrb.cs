using System;
using UnityEngine;

namespace Mush
{
    public class MushOrb :MonoBehaviour
    {
        [SerializeField] public float launcForceX;
        [SerializeField] public float launcForceY;
        private Rigidbody2D rb;
        private Vector3 startPos;

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
                gameObject.SetActive(false);
            }

            if (other.CompareTag("Player"))
            {
                DoDamage.DealDamage();
                other.GetComponent<Animator>().SetTrigger("Damage");
            }
        }


    }
}