using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Campfire : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string campfireID;

    private Transform _fire;
    private Light2D _fireLight;
    private Transform _player;
    private bool _isMoving;

    [SerializeField] private float scaleGrowth = 0.001f;
    [SerializeField] private float timeInterval = 0.1f;

    void Start()
    {
        _fireLight = GetComponentInChildren<Light2D>();
        _fire = _fireLight.transform.parent;

        _fire.localScale = new Vector3(0.1f, 0.1f, 1f);
        _fireLight.enabled = false;
        
        LoadState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !_isMoving && _player != null)
        {
            StartCoroutine(MovePlayerToOffset());
        }
    }

    private void LoadState()
    {
        if (SaveSystem.CurrentData.activatedCampfires.Contains(campfireID))
        {
            _fireLight.enabled = true;
            _fire.localScale = Vector3.one * 3f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _player = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _player = null;
    }

    private IEnumerator MovePlayerToOffset()
    {
        
        Animator playerAnim = _player.GetComponent<Animator>();
        Rigidbody2D playerRb = _player.GetComponent<Rigidbody2D>();
        PlayerController playerController = _player.GetComponent<PlayerController>();
        if (playerAnim.GetBool("isResting"))
        {
            playerAnim.SetBool("isResting", false);
            playerController.canMove = true;
            yield break;
        }

        _isMoving = true;
        playerController.canMove = false;
        float directionToPlayer = _player.position.x > transform.position.x ? 1.0f : -1.0f;
        float targetX = transform.position.x + directionToPlayer;

        playerAnim.SetBool("IsRunning", true);

        float stopThreshold = 0.05f;

        // 2. Movement Loop
        while (Mathf.Abs(_player.position.x - targetX) > stopThreshold)
        {
            // Determine direction to the TARGET point
            float moveDir = _player.position.x > targetX ? -1f : 1f;
            playerRb.linearVelocity = new Vector2(moveDir * playerController.speed, playerRb.linearVelocity.y);

            _player.transform.localScale = moveDir>0 ? 
                new Vector2(Math.Abs(_player.transform.localScale.x), _player.transform.localScale.y) : 
                new Vector2(-Math.Abs(_player.transform.localScale.x), _player.transform.localScale.y);

            yield return null;
        }

        // 3. Cleanup & Snap
        playerRb.linearVelocity = Vector2.zero;
        _player.position = new Vector3(targetX, _player.position.y, _player.position.z);
        playerAnim.SetBool("IsRunning", false);

        playerAnim.SetBool("isResting", true);
        ActivateCampfire(_player.GetComponent<Collider2D>());
        _isMoving = false;
    }

    private void ActivateCampfire(Collider2D player)
    {
        _fireLight.enabled = true;

        SaveSystem.CurrentData.lastScene = SceneManager.GetActiveScene().name;
        SaveSystem.CurrentData.playerPos = player.transform.position;
    
        if (Camera.main != null)
        {
            SaveSystem.CurrentData.cameraPos = Camera.main.transform.position;
        }

        ParallaxLayer[] layers = FindObjectsByType<ParallaxLayer>(sortMode:FindObjectsSortMode.InstanceID);
        foreach (ParallaxLayer layer in layers)
        {
            layer.SaveState();
        }

        if (!SaveSystem.CurrentData.activatedCampfires.Contains(campfireID))
        {
            SaveSystem.CurrentData.activatedCampfires.Add(campfireID);
        }
        
        string folderPath = Path.GetDirectoryName(SaveSystem.SavePath);
        string saveName = Path.GetFileNameWithoutExtension(SaveSystem.SavePath);
        
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        
        string screenshotPath = Path.Combine(folderPath, saveName + ".png");
        ScreenCapture.CaptureScreenshot(screenshotPath);
        
        SaveSystem.CurrentData.isNewGame = false;
        SaveSystem.SaveToFile();

        player.GetComponent<PlayerController>().playerHealth = 90;
        StartCoroutine(GrowFire());
    }

    private IEnumerator GrowFire()
    {
        while (_fire.localScale.x < 3f)
        {
            _fire.localScale += Vector3.one * scaleGrowth;
            yield return new WaitForSeconds(timeInterval);
        }
    }
}