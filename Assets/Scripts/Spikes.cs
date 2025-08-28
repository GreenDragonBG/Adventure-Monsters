using System;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private float lastDamageTime= -Mathf.Infinity;
    private float damageCooldown = 0.1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ((Time.time - lastDamageTime) >= damageCooldown))
        {
            lastDamageTime = Time.time;
            SpikeDamage();
        }
    }

    private static void SpikeDamage()
    {
        if (DoDamage._playerController.PlayerHealth>30)
        {
            Checkpoint.Respawn();
        }
        DoDamage.DealDamage();
    }
}
