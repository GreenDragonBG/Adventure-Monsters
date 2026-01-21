using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class BossGate : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string gateID;
    
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
    private List<Light2D> lights = new List<Light2D>();

    private void Start()
    {
        LoadState();
        
        if (Camera.main != null) cameraShake = Camera.main.GetComponent<CameraShake>();
        startYPosition = gate.transform.localPosition.y;

        // Better way to find all lights even if nested
        if (lightsExist && lightsLayer != null)
        {
            lights.AddRange(lightsLayer.GetComponentsInChildren<Light2D>(true));
        }

        if (hasFinished)
        {
            // Snap gate to final position
            Vector3 finalPos = gate.transform.localPosition;
            finalPos.y = finalYPosition;
            gate.transform.localPosition = finalPos;

            // Enable boss logic
            boss.enabled = true;

            // Prepare lights to fade in (don't snap intensity to 1)
            if (lightsExist)
            {
                foreach (Light2D item in lights) item.intensity = 0f;
                lightsLayer.SetActive(true);
            }
        }
        else
        {
            boss.enabled = false;
            if (lightsExist)
            {
                foreach (Light2D item in lights) item.intensity = 0f;
                lightsLayer.SetActive(false);
            }
        }
    }

    void Update()
    {
        // 1. LIGHT LOGIC (Outside of the return so it runs even if hasFinished is true)
        HandleLightFading();

        // 2. GATE RETRACTION (If player dies)
        if (!boss.enabled && gate.transform.localPosition.y > startYPosition)
        { 
            MoveBack(gate);
        }

        // 3. GATE MOVEMENT LOGIC
        if (hasFinished) return;

        if (startMoving && gate.transform.localPosition.y < finalYPosition)
        {
            Move(gate);
            bossBar.gameObject.SetActive(true);
            if(lightsExist) lightsLayer.SetActive(true);
        }
        else if (startMoving)
        {
            hasFinished = true;
            SaveFinishedState();
            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.3f, 0.1f));
        }
    }

    private void HandleLightFading()
    {
        // If the lights should be visible (gate is moving OR gate is already finished)
        bool shouldBeOn = (startMoving || hasFinished);

        if (lightsExist && shouldBeOn && lights.Count > 0)
        {
            foreach (Light2D item in lights)
            {
                if (item.intensity < 2f)
                {
                    // Using MoveTowards for a clean, non-frame-rate-dependent transition
                    item.intensity = Mathf.MoveTowards(item.intensity, 2f, lightSpeed * Time.deltaTime);
                }
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasFinished && other.CompareTag("Player"))
        {
            boss.enabled = true;
            startMoving = true;
        }
    }

    private void Move(Tilemap tilemap)
    {
        Vector3 newPos = tilemap.transform.localPosition;
        newPos.y += moveSpeed * Time.deltaTime;
        tilemap.transform.localPosition = newPos;
    }

    private void MoveBack(Tilemap tilemap)
    {
        Vector3 newPos = tilemap.transform.localPosition;
        newPos.y -= moveSpeed * Time.deltaTime;
        tilemap.transform.localPosition = newPos;
    }
    
    private void SaveFinishedState()
    {
        if (!SaveSystem.CurrentData.finishedGates.Contains("BossGate_"+gateID))
        {
            SaveSystem.CurrentData.finishedGates.Add("BossGate_"+gateID);
        }
    }

    private void LoadState()
    {
        if (SaveSystem.CurrentData.finishedGates.Contains("BossGate_"+gateID))
        {
            hasFinished = true;
        }
    }
}