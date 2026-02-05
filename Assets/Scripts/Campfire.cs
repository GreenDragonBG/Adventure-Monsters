using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Campfire : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string campfireID;

    private Transform fire;
    private Light2D fireLight;

    [SerializeField] private float scaleGrowth = 0.001f;
    [SerializeField] private float timeInterval = 0.1f;

    void Start()
    {
        fireLight = GetComponentInChildren<Light2D>();
        fire = fireLight.transform.parent;

        fire.localScale = new Vector3(0.1f, 0.1f, 1f);
        fireLight.enabled = false;

        LoadState();
    }

    private void LoadState()
    {
        // Check our JSON list instead of PlayerPrefs
        if (SaveSystem.CurrentData.activatedCampfires.Contains(campfireID))
        {
            fireLight.enabled = true;
            fire.localScale = Vector3.one * 3f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            ActivateCampfire(other);
        }
    }

    private void ActivateCampfire(Collider2D player)
    {
        fireLight.enabled = true;

        // 1. Update Player & Scene Data in RAM
        SaveSystem.CurrentData.lastScene = SceneManager.GetActiveScene().name;
        SaveSystem.CurrentData.playerPos = player.transform.position;
    
        // 2. Update Camera Data in RAM
        if (Camera.main != null)
        {
            SaveSystem.CurrentData.cameraPos = Camera.main.transform.position;
        }

        // 3. Collect Parallax Data
        // We find all layers and tell them to update the lists in SaveSystem.CurrentData
        ParallaxLayer[] layers = FindObjectsByType<ParallaxLayer>(sortMode:FindObjectsSortMode.InstanceID);
        foreach (ParallaxLayer layer in layers)
        {
            layer.SaveState();
        }

        // 4. Update this specific campfire's status
        if (!SaveSystem.CurrentData.activatedCampfires.Contains(campfireID))
        {
            SaveSystem.CurrentData.activatedCampfires.Add(campfireID);
        }
        
        string folderPath = Path.GetDirectoryName(SaveSystem.SavePath);
        string saveName = Path.GetFileNameWithoutExtension(SaveSystem.SavePath);
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        string screenshotPath = Path.Combine(folderPath, saveName + ".png");
        
        ScreenCapture.CaptureScreenshot(screenshotPath);
        
        SaveSystem.CurrentData.isNewGame = false;
        SaveSystem.SaveToFile();

        // Visuals/Gameplay
        player.GetComponent<PlayerController>().playerHealth = 90;
        StartCoroutine(GrowFire());
    }

    private IEnumerator GrowFire()
    {
        while (fire.localScale.x < 3f)
        {
            fire.localScale += Vector3.one * scaleGrowth;
            yield return new WaitForSeconds(timeInterval);
        }
    }
}