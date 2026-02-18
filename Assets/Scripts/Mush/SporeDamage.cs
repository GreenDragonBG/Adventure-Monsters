using System;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField] public bool willTeleport;
    private new ParticleSystem particleSystem;
    private static readonly float DamageCooldown = 1f;
    
    private ParticleSystem.CollisionModule particleCollision;

    private static float _lastDamageTime = -Mathf.Infinity;

    void OnParticleCollision(GameObject other)
    {
        // Only damage the player
        if (other.CompareTag("Player"))
        {
            particleCollision.enabled = false;
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
        if (! particleCollision.enabled && Time.time - _lastDamageTime >= DamageCooldown)
        {
            particleCollision.enabled = true;
        }
    }

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleCollision = particleSystem.collision;
        particleCollision.type = ParticleSystemCollisionType.World;
    }
}