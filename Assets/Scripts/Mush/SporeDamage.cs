using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField] public bool willTeleport;
    private ParticleSystem particleSystem;
    private float damageCooldown = 6f;

    private float lastDamageTime = -Mathf.Infinity;

    void OnParticleCollision(GameObject other)
    {
        // Only damage the player
        if (other.CompareTag("Player"))
        {
            // Check if cooldown has passed
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                lastDamageTime = Time.time;

                if (willTeleport)
                {
                    Checkpoint.Respawn();
                }
                
                DoDamage.DealDamage();
            }
        }
    }

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        var collision = particleSystem.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        particleSystem.Stop();
    }
}