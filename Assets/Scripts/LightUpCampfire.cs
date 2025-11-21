using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightUpCampfire : MonoBehaviour
{
    private Transform fire;
    private Light2D fireLight;
    [SerializeField] private float scaleGroweth = 0.001f;
    [SerializeField] private float timeInterval = 0.1f;
    private float timeInbetweenGrowth = -Mathf.Infinity;
    private bool isTriggered;

    void Start()
    {
        fireLight = GetComponentInChildren<Light2D>();
        fire = fireLight.transform.parent;
        if (fire!=null)
        {
            fire.localScale = new Vector3(0.1f, 0.1f, 1f);
        }

        isTriggered = false;
        fireLight.enabled = false;
    }

    void Update()
    {
        if (isTriggered && Time.time-timeInbetweenGrowth > timeInterval && fire.localScale.x < 2)
        {
            fire.localScale = new Vector3(fire.localScale.x+scaleGroweth,fire.localScale.y+scaleGroweth, 1f);
            timeInbetweenGrowth = Time.time;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            fireLight.enabled = true;
        }
    }
}
