using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField] public bool willTeleport;
    private ParticleSystem _particleSystem;
    private static readonly float DamageCooldown = 1f;
    
    private ParticleSystem.CollisionModule _particleCollision;

    private static float _lastDamageTime = -Mathf.Infinity;

    void OnParticleCollision(GameObject other)
    {
        // Only damage the player
        if (other.CompareTag("Player"))
        {
            _particleCollision.enabled = false;
            // Check if cooldown has passed
            if (Time.time - _lastDamageTime >= DamageCooldown)
            {
                _lastDamageTime = Time.time;

                if (willTeleport)
                {
                    Checkpoint.Respawn();
                }
                
                DoDamage.DealDamage();
            }
        }
    }

    private void Update()
    {
        if (!_particleCollision.enabled && Time.time - _lastDamageTime >= DamageCooldown)
        {
            _particleCollision.enabled = true;
        }
    }

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleCollision = _particleSystem.collision;
        _particleCollision.type = ParticleSystemCollisionType.World;
    }
}