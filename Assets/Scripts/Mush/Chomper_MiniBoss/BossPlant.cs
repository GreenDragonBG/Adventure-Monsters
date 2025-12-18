using UnityEngine;

public class BossPlants : ExtendedPlant
{
    private bool playerInside = false;
    
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = true;
    
                if (CanAttack)
                {
                    PlayerAnimator = other.GetComponent<Animator>();
                    StartCoroutine(Attack());
                }
            }
        }
    
        protected void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = false;
            }
        }
    
        protected new void DealTheDamage()
        {
            if (!playerInside)
                return;
    
            base.DealTheDamage();
        }
}
