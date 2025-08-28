using System;
using UnityEngine;

public class ChargeWall : MonoBehaviour
{
    private ParticleSystem particle;
    private CameraShake cameraShake;
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.Stop();
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            particle.Play();
            StartCoroutine(cameraShake.Shake(1.5f, 0.2f));
            Debug.Log("TriggerA entered another trigger: " + other.name);
            other.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            other.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
