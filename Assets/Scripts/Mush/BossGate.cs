using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
public class BossGate : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lightSpeed;
    [SerializeField] private Tilemap gate;
    [SerializeField] private GameObject layer;
    [SerializeField] private MushBoss boss;
    private bool startMoving = false;
    private List<Light2D> lights;

    //Makes the boss not active , turns the lights off and make their intensity to 0 
    private void Start()
    {
        boss.enabled = false;
        lights = new List<Light2D>();
        for (int i =0; i<layer.transform.childCount; i++)
        {
            Light2D item = layer.transform.GetChild(i).GetComponent<Light2D>();
            lights.Add(item);
        }

        foreach (Light2D item in lights)
        {
            item.intensity = 0f;
        }
        
        layer.SetActive(false);
    }

    void Update()
    {
        //moves the gate
        if (startMoving && gate.transform.localPosition.y < 0f)
        {
            Move(gate);
            layer.SetActive(true);
        }
        
        //slowly rises the light's intensity
        if (startMoving && lights[0].intensity < 1 )
        {
            foreach (Light2D item in lights)
            {
                item.intensity += lightSpeed;
                
            }
        }
    }
    
    //registers that we have triggered the zone and turns the boss script on
    private void OnTriggerEnter2D(Collider2D other)
    {
        boss.enabled = true;
        startMoving = true;
    }

    //moves the gate
    public void Move(Tilemap tilemap)
    {
        Vector3 newPos = tilemap.transform.localPosition;
        newPos.y += moveSpeed;
 
        tilemap.transform.localPosition = newPos;
    }
}
