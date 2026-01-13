using UnityEngine;

public class TimedSpores : MonoBehaviour
{
    private ParticleSystem poisonParticles;
    
    private void Awake()
    {
        poisonParticles = GetComponent<ParticleSystem>();
        poisonParticles.Stop();
    }
    

    // --- Called by animation event ---
    public void PoisonAttack()
    {
        poisonParticles.Play();
    }
}
