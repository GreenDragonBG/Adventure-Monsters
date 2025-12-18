using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightUpCampfire : MonoBehaviour
{
    private Transform fire;
    private Light2D fireLight;
    
    [SerializeField] private float scaleGrowth = 0.001f;
    [SerializeField] private float timeInterval = 0.1f;
    
    private bool isTriggered = false;

    void Start()
    {
        fireLight = GetComponentInChildren<Light2D>();
        fire = fireLight.transform.parent;

        if (fire != null)
        {
            fire.localScale = new Vector3(0.1f, 0.1f, 1f);
        }

        fireLight.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            fireLight.enabled = true;
            
            StartCoroutine(GrowFire());
        }
    }

    private IEnumerator GrowFire()
    {
        while (fire.localScale.x < 3f)
        {
            fire.localScale += new Vector3(scaleGrowth, scaleGrowth, 0f);
            
            yield return new WaitForSeconds(timeInterval);
        }
    }
}
