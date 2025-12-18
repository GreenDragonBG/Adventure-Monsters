using System;
using UnityEngine;

public class Thorne : MonoBehaviour
{
    private float minDistanceBetween;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") //damage player
        {
            DoDamage.DealDamage();
            other.GetComponent<Animator>().SetTrigger("Damage");
        } 
        else if (other.GetComponent<Thorne>()!=null) // destroy itself if it overlaps another one 
        {
            if ((other.transform.position.x > transform.position.x && (other.transform.position.x- transform.position.x)<minDistanceBetween)
                ||
                (other.transform.position.x < transform.position.x && (transform.position.x-other.transform.position.x)<minDistanceBetween))
            {
                Destroy(gameObject);
            }
        }
    }
}
