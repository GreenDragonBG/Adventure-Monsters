using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace Mush
{
    public class MushOrb :MonoBehaviour
    {
        [SerializeField] public float timeTillLand;
        [SerializeField] public Transform playerPosition;
        [SerializeField] public float launchDelay;
        private MushBoss boss;
        public bool toLaunch = false;
        private Rigidbody2D rb;
        private Vector2 startPos;
        
        private float timeStartedDelay = -Mathf.Infinity;

        private void Start()
        {
            //finds Rigidbody2D and it's starting position
            rb = GetComponent<Rigidbody2D>();
            startPos = transform.position;
        }

        private void Update()
        { 
            if (toLaunch && Time.time - timeStartedDelay >=launchDelay)
            {
                toLaunch = false;
                Launch();
            }
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

        private void Launch()
        {
            Vector2 endPos = playerPosition.position;
            float g = Physics2D.gravity.y;
            
            Vector2 distance = endPos - startPos;
            
            float Vx = distance.x / timeTillLand;
            float Vy = ( distance.y / timeTillLand) + (0.5f * Mathf.Abs(Physics2D.gravity.y)  * timeTillLand);
            
            boss.isAttacking = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = new Vector2(Vx,Vy);
        }

        public void StartLaunch(MushBoss boss)
        {
            this.boss = boss;
            rb.bodyType = RigidbodyType2D.Static;
            timeStartedDelay = Time.time;
            toLaunch = true;
        }

    }
}