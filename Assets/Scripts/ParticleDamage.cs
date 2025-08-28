using System;
using UnityEngine;

public class Parti : MonoBehaviour
{
    [SerializeField]private GameObject player;
    private Rigidbody2D playerRb;
    private float hitCooldown = 1.0f;
    private float lastHitTime = -999f;
    void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other == player)
        {
            if (Time.time - lastHitTime < hitCooldown)
                return;

            lastHitTime = Time.time;

            DoDamage.DealDamage();
        }
    }

    public Vector2 CalculateLaunchVelocity(Vector2 start, Vector2 target)
    {
        float timeToTarget = 0.75f;
        hitCooldown = timeToTarget;
        float gravity = 3;
        Vector2 delta = target - start;

        float vx = delta.x / timeToTarget;
        float vy = (delta.y / timeToTarget) + (0.5f * gravity * timeToTarget);

        return new Vector2(vx, vy);
    }

}
