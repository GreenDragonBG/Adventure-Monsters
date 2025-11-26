using UnityEngine;

public class PoisonMushroom : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    public static float GlobalAttackDelay = 4f; // Shared delay between global attacks
    private static float _lastGlobalAttackTime = -Mathf.Infinity;
    private float localLastAttackTime = -Mathf.Infinity; // Each mushroom remembers when it last attacked
    private ParticleSystem poisonParticles;
    
    private void Awake()
    {
        poisonParticles = GetComponent<ParticleSystem>();
        poisonParticles.Stop();
    }

    private void Update()
    {
        // Check if it's time for a new global attack
        if (Time.time - _lastGlobalAttackTime >= GlobalAttackDelay)
        {
            _lastGlobalAttackTime = Time.time;
        }

        // If this mushroom hasn't attacked yet this cycle, trigger its animation
        if (localLastAttackTime < _lastGlobalAttackTime)
        {
            animator.SetTrigger("Attack");
            localLastAttackTime = Time.time;
        }
    }
    

    // --- Called by animation event ---
    public void PoisonAttack()
    {
        poisonParticles.Play();
    }
}
