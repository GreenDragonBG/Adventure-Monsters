using System;
using UnityEngine;

public class WeponScript : MonoBehaviour
{
    private float timeSinceAttacked = -Mathf.Infinity;
    private void Update()
    {
        if (Time.time - timeSinceAttacked > 1f)
        {
            Swordman.isAttacking = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        
        if (Swordman.isAttacking)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
            timeSinceAttacked = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyScript>().health -= 50;
            if ( other.GetComponent<EnemyScript>().health <=0)
            {
                Vector3 newScale = other.transform.localScale;
                newScale.y *= -1;
                other.transform.localScale = newScale;
                other.GetComponent<EnemyScript>().TimeFromDead = Time.time;
            }
        }
    }
}
