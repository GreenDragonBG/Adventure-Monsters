using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Seed : MonoBehaviour
{
    
    [SerializeField] private GameObject thornePrefab;
    [SerializeField] private float thorneYPosition;
    private Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    { 
        if (other.name == "SpikeSurface")
        {
            Vector2 impactPos = rb2d.transform.position;

            GameObject spike =Instantiate(thornePrefab, impactPos, Quaternion.identity);
            spike.transform.position = new Vector2(impactPos.x, thorneYPosition);
            returnBack();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DoDamage.DealDamage();
            other.gameObject.GetComponent<Animator>().SetTrigger("Damage");
            returnBack();
        }
    }

    private void returnBack()
    {
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        transform.position = transform.parent.position;
        gameObject.SetActive(false);
    }
}
