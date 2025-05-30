using System;
using UnityEngine;

namespace Mush
{
    public class MushOrb :MonoBehaviour
    {
        public float launcForceX = 2f;
        public float launcForceY = 10f;
        private Rigidbody2D rb;
        private Vector3 startPos;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startPos = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.name == "OrbDeadZone")
            {
                transform.position = startPos;
                gameObject.SetActive(false);
            }
        }

        public static void LaunchAll(MushOrb[] orbs)
        {
            foreach (MushOrb orb in orbs)
            {
                orb.gameObject.SetActive(true);
                orb.rb.linearVelocity = new Vector2(orb.launcForceX, orb.launcForceY);
            }
        }
    }
}