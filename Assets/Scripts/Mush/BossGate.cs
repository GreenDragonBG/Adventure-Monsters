using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
public class BossGate : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private int gateID = 1;
    
    [Header("Boss")]
    [SerializeField] private MonoBehaviour boss;
    [SerializeField] private BossBar bossBar;
    
    [Header("Gate")]
    [SerializeField] private float finalYPosition;
    private float startYPosition;
    [SerializeField] private Tilemap gate;
    [SerializeField] private float moveSpeed;
    CameraShake cameraShake;
    private bool startMoving;
    private bool hasFinished;
    
    [Header("Lights")]
    [SerializeField] private bool lightsExist;
    [SerializeField] private GameObject lightsLayer;
    [SerializeField] private float lightSpeed;
    private List<Light2D> lights;

    //Makes the boss not active , turns the lights off and make their intensity to 0 
    private void Start()
    {
        LoadState();
        
        if (Camera.main != null) cameraShake = Camera.main.GetComponent<CameraShake>();
        
        if(!hasFinished)
         boss.enabled = false;
        
        if (!lightsExist) return;
        
        lights = new List<Light2D>();
        for (int i =0; i<lightsLayer.transform.childCount; i++)
        {
            Light2D item = lightsLayer.transform.GetChild(i).GetComponent<Light2D>();
            lights.Add(item);
        }
            
        foreach (Light2D item in lights)
        {
            item.intensity = 0f;
        }
            
        lightsLayer.SetActive(false);
        
        startYPosition = gate.transform.localPosition.y;
    }

    void Update()
    {
        if (!boss.enabled && gate.transform.localPosition.y > startYPosition)
        { 
            MoveBack(gate);
            if (hasFinished)
            {
                SaveFinishedState();
            }
        }

        if(hasFinished) return;
        //moves the gate
        if (startMoving && gate.transform.localPosition.y < finalYPosition)
        {
            Move(gate);
            bossBar.gameObject.SetActive(true);
            if(lightsExist) lightsLayer.SetActive(true);
        }else if (startMoving)
        {
            hasFinished = true;
            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.3f, 0.1f));
        }

        //slowly rises the light's intensity
        if (lightsExist && startMoving && lights[0].intensity < 1 )
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
        if (!hasFinished && other.CompareTag("Player"))
        {
            boss.enabled = true;
            startMoving = true;
        }
    }

    //moves the gate
    private void Move(Tilemap tilemap)
    {
        Vector3 newPos = tilemap.transform.localPosition;
        newPos.y += moveSpeed;
 
        tilemap.transform.localPosition = newPos;
    }

    private void MoveBack(Tilemap tilemap)
    {
        Vector3 newPos = tilemap.transform.localPosition;
        newPos.y -= moveSpeed;
 
        tilemap.transform.localPosition = newPos;
    }
    
    private void SaveFinishedState()
    {
        if (!Application.isPlaying)
            return;
        
        PlayerPrefs.SetInt("GateClosed_"+ gateID,1);
    }

    private void LoadState()
    {
        if (!PlayerPrefs.HasKey("GateClosed_"+ gateID) || !Application.isPlaying)
            return;
        
        if (PlayerPrefs.GetInt("GateClosed_"+ gateID) == 1)
        {
            hasFinished = true;
        }
    }
}
