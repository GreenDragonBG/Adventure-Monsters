using System;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private float lastDamageTime= -Mathf.Infinity;
    private float damageCooldown = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ((Time.time - lastDamageTime) >= damageCooldown))
        {
            lastDamageTime = Time.time;
            SpikeDamage();
        }
    }

    public  static void SpikeDamage()
    {  
        Debug.Log("SpikeDamage");
        Checkpoint.Respawn();
        DoDamage.DealDamage();
    }
}
