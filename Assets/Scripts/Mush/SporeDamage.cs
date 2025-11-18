using System;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    [SerializeField] public bool willTeleport;
    private ParticleSystem particleSystem;
    private static float _damageCooldown = 4f;
    
    private ParticleSystem.CollisionModule particleCollision;

    private static float _lastDamageTime = -Mathf.Infinity;

    void OnParticleCollision(GameObject other)
    {
        // Only damage the player
        if (other.CompareTag("Player"))
        {
            particleCollision.enabled = false;
            // Check if cooldown has passed
            if (Time.time - _lastDamageTime >= _damageCooldown)
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
        if(!particleSystem.isPlaying){
            particleCollision.enabled = true;
        }
    }

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleCollision = particleSystem.collision;
        particleCollision.type = ParticleSystemCollisionType.World;
        particleSystem.Stop();
    }
}