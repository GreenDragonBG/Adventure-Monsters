using UnityEngine;

public class BossPlants : ExtendedPlant
{
    
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerAnimator = other.GetComponent<Animator>();
                DealTheDamage();
            }
        }
}
