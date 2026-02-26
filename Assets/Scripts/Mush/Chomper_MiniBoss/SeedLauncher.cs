using System;
using UnityEngine;

public class SeedLauncher : MonoBehaviour
{
    [SerializeField] private float timeTillLand;
    [SerializeField] private Transform playerPosition;
    private GameObject seed;
    private Rigidbody2D seedRb;
    
    void Start()
    {
        seedRb = GetComponentInChildren<Rigidbody2D>();
        seed =seedRb.gameObject;
        seedRb.bodyType = RigidbodyType2D.Kinematic;
        seed.SetActive(false);
    }
    
    public void LaunchSeed()
    {
        Vector2 endPos = playerPosition.position;
        Vector2 startPos = seedRb.transform.position;
        float g = Physics2D.gravity.y;
            
        Vector2 distance = (endPos - startPos);
            
        float vx = distance.x / timeTillLand;
        float vy = ( distance.y / timeTillLand) + (0.5f * Mathf.Abs(Physics2D.gravity.y)  * timeTillLand);
        
        seed.SetActive(true);
        seedRb.bodyType = RigidbodyType2D.Dynamic;
        seedRb.linearVelocity = new Vector2(vx,vy);
    }
}
