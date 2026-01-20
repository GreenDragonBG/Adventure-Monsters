using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class BossGate : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string gateID; // Changed to string for the list
    
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

    private void Start()
    {
        // 1. Load data from JSON
        LoadState();
        
        if (Camera.main != null) cameraShake = Camera.main.GetComponent<CameraShake>();
        startYPosition = gate.transform.localPosition.y;

        // Initialize lights list if they exist
        if (lightsExist)
        {
            lights = new List<Light2D>();
            for (int i = 0; i < lightsLayer.transform.childCount; i++)
            {
                Light2D item = lightsLayer.transform.GetChild(i).GetComponent<Light2D>();
                if (item != null) lights.Add(item);
            }
        }

        // 2. If already finished, snap the gate and setup state
        if (hasFinished)
        {
            if (lightsExist)
            {
                lightsLayer.SetActive(true);
                foreach (Light2D item in lights) item.intensity = 1f;
            }
            // If the gate is finished, the boss should probably be active 
            // (or dead, which the boss script handles itself)
            boss.enabled = true; 
        }
        else
        {
            // Fresh state for a gate not yet triggered
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
        // If boss is disabled (player died/lost), move gate back
        // We only save this if it's the "closing" logic you intended
        if (!boss.enabled && gate.transform.localPosition.y > startYPosition)
        { 
            MoveBack(gate);
        }

        if (hasFinished) return;

        // Moves the gate UP when triggered
        if (startMoving && gate.transform.localPosition.y < finalYPosition)
        {
            Move(gate);
            bossBar.gameObject.SetActive(true);
            if(lightsExist) lightsLayer.SetActive(true);
        }
        else if (startMoving)
        {
            hasFinished = true;
            SaveFinishedState(); // SAVE TO RAM
            if (cameraShake != null)
                StartCoroutine(cameraShake.Shake(0.3f, 0.1f));
        }

        // Slowly rises the light's intensity
        if (lightsExist && startMoving && lights.Count > 0 && lights[0].intensity < 1)
        {
            foreach (Light2D item in lights)
            {
                item.intensity += lightSpeed * Time.deltaTime; // Added Time.deltaTime for smoothness
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
        newPos.y += moveSpeed * Time.deltaTime; // Use deltaTime so speed is consistent
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
            // Note: We don't call SaveToFile here because we wait for the Campfire!
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