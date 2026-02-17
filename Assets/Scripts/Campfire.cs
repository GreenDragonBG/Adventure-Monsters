using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Campfire : MonoBehaviour
{
    private static readonly int IsResting = Animator.StringToHash("isResting");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");

    [Header("ID & Settings")]
    [SerializeField] private string campfireID;
    [SerializeField] private float targetFireScale = 3f;
    [SerializeField] private float scaleGrowthSpeed = 2f;
    [SerializeField] private float stopThreshold = 0.05f;
    [SerializeField] private TextMeshProUGUI savedText;

    [Header("Fire")]
    private Transform _fireTransform;
    private Light2D _fireLight;

    [Header("Save spots")] 
    [SerializeField] private GameObject healPrefab;
    private Coroutine _saveSpotCoroutine;
    [SerializeField] private SpriteRenderer leftSaveSpot;
    private Light2D _leftLight;
    [SerializeField] private SpriteRenderer rightSaveSpot;
    private Light2D _rightLight;

    private Transform _playerTransform;
    private PlayerController _playerController;
    private Rigidbody2D _playerRb;
    private Animator _playerAnim;
    
    private bool _isMoving;
    private Camera _camera;
    private CameraShake  _cameraShake;

    private void Start()
    {
        _camera = Camera.main;
        _cameraShake = _camera?.GetComponent<CameraShake>();
        InitializeCampfire();
        LoadState();
    }

    private void Update()
    {
        if (_playerTransform)
        {
            _saveSpotCoroutine=StartCoroutine(!_playerAnim.GetBool(IsResting) ? ShowSaveSpots() : HideSaveSpots());
        }
        else
        {
            _saveSpotCoroutine=StartCoroutine(HideSaveSpots());
        }

        if (Input.GetKeyDown(KeyCode.C) && !_isMoving && _playerTransform)
        {
            StartCoroutine(HandleRestingInteraction());
        }

        if (_playerTransform && _playerAnim.GetBool(IsResting) && _playerController.playerHealth==90)
        {
            StartCoroutine(HandleRestingInteraction());
        }
    }

    private void InitializeCampfire()
    {
        _fireLight = GetComponentInChildren<Light2D>();
        _fireTransform = _fireLight.transform.parent;

        _fireTransform.localScale = new Vector3(0.1f, 0.1f, 1f);
        _fireLight.enabled = false;

        _leftLight = leftSaveSpot.GetComponentInChildren<Light2D>();
        _rightLight = rightSaveSpot.GetComponentInChildren<Light2D>();
    }

    private IEnumerator ShowSaveSpots()
    {
        if (_saveSpotCoroutine!=null)
        {
            StopCoroutine(_saveSpotCoroutine);
        }

        Color tempColor = leftSaveSpot.color;

        float tempLight = _leftLight.intensity;

        while (tempColor.a<1)
        {
            tempColor.a += 0.02f;
            tempLight += 0.02f;

            leftSaveSpot.color = tempColor;
            rightSaveSpot.color = tempColor;

            _leftLight.intensity = tempLight;
            _rightLight.intensity = tempLight;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator HideSaveSpots()
    { 
        if (_saveSpotCoroutine!=null)
        {
            StopCoroutine(_saveSpotCoroutine);
        }

        Color tempColor = leftSaveSpot.color;

        float tempLight = _leftLight.intensity;

        while (tempColor.a>0)
        {
            tempColor.a -= 0.02f;
            tempLight -= 0.02f;

            leftSaveSpot.color = tempColor;
            rightSaveSpot.color = tempColor;

            _leftLight.intensity = tempLight;
            _rightLight.intensity = tempLight;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void LoadState()
    {
        if (SaveSystem.CurrentData.activatedCampfires.Contains(campfireID))
        {
            _fireLight.enabled = true;
            _fireTransform.localScale = Vector3.one * targetFireScale;
        }
    }

    private IEnumerator HandleRestingInteraction()
    {
        // Toggle Resting Off
        if (_playerAnim.GetBool(IsResting))
        {
            SetRestingState(false);
            yield break;
        }

        // Move to Fire and Rest
        _isMoving = true;
        _playerController.canMove = false;

        yield return MovePlayerToPosition();

        SetRestingState(true);
        ActivateCampfire();
        
        _isMoving = false;
    }

    private IEnumerator MovePlayerToPosition()
    {
        float directionToPlayer = _playerTransform.position.x > transform.position.x ? 1.0f : -1.0f;
        float targetX = transform.position.x + directionToPlayer;

        _playerAnim.SetBool(IsRunning, true);

        while (Mathf.Abs(_playerTransform.position.x - targetX) > stopThreshold)
        {
            float moveDir = _playerTransform.position.x > targetX ? -1f : 1f;
            _playerRb.linearVelocity = new Vector2(moveDir * _playerController.speed, _playerRb.linearVelocity.y);

            FlipPlayer(moveDir);
            yield return null;
        }

        // Snap to finish
        _playerRb.linearVelocity = Vector2.zero;
        _playerTransform.position = new Vector3(targetX, _playerTransform.position.y, _playerTransform.position.z);
        _playerAnim.SetBool(IsRunning, false);
    }

    private void SetRestingState(bool isResting)
    {
        if (isResting) StartCoroutine(ShowSavedText());
        _playerAnim.SetBool(IsResting, isResting);
        _playerController.canMove = !isResting;
    }

    private void ActivateCampfire()
    {
        _fireLight.enabled = true;

        SaveProgress();
        StartCoroutine(GrowFireRoutine());
        StartCoroutine(HealOverTime());
    }

    private IEnumerator HealOverTime()
    {
        yield return new WaitForSeconds(3f);
        while (_playerTransform && _playerAnim.GetBool(IsResting) && _playerController.playerHealth < 90)
        {
            if (_playerController.playerHealth<60)
            {
                _playerController.playerHealth += 30;
            }
            else
            {
                _playerController.playerHealth = 90;
            }

            GameObject healEffect = Instantiate(healPrefab,
                _playerTransform.position.x < transform.position.x
                    ? new Vector2(transform.position.x - 1f, transform.position.y - 0.2f)
                    : new Vector2(transform.position.x + 1f, transform.position.y - 0.2f)
                , Quaternion.identity);
            StartCoroutine(_cameraShake.Shake(0.6f, 0.025f));
            StartCoroutine(HealEffectStart(healEffect));
            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator HealEffectStart(GameObject healEffect)
    {
        Light2D light2D = healEffect.GetComponentInChildren<Light2D>();
        float tempIntensity = light2D.intensity;
        float tempOuter = light2D.pointLightOuterRadius;

        while (tempOuter<10)
        {
            yield return new WaitForSeconds(0.01f);
            tempIntensity += 0.1f;
            tempOuter += 0.2f;
            light2D.intensity = tempIntensity;
            light2D.pointLightOuterRadius = tempOuter;
        }
        StartCoroutine(HealEffectFinish(healEffect,light2D));
    }
    
    private IEnumerator HealEffectFinish(GameObject healEffect, Light2D light2D)
    {
        float tempIntensity = light2D.intensity;

        while (tempIntensity>0)
        {
            yield return new WaitForSeconds(0.005f);
            tempIntensity -= 0.1f;
            light2D.intensity = tempIntensity;
        }
        Destroy(healEffect);
    }

    private IEnumerator ShowSavedText()
    {
        savedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        savedText.gameObject.SetActive(false);
    }

    private void SaveProgress()
    {
        var data = SaveSystem.CurrentData;
        data.lastScene = SceneManager.GetActiveScene().name;
        data.playerPos = _playerTransform.position;
        data.isNewGame = false;

        data.cameraPos = _camera.transform.position;

        // Save Parallax
        var layers = FindObjectsByType<ParallaxLayer>(FindObjectsSortMode.InstanceID);
        foreach (var layer in layers) layer.SaveState();

        // ID Management
        if (!data.activatedCampfires.Contains(campfireID))
            data.activatedCampfires.Add(campfireID);

        // Screenshot & File IO
        CaptureSaveScreenshot();
        SaveSystem.SaveToFile();
    }

    private void CaptureSaveScreenshot()
    {
        string folderPath = Path.GetDirectoryName(SaveSystem.SavePath);
        string saveName = Path.GetFileNameWithoutExtension(SaveSystem.SavePath);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        
        ScreenCapture.CaptureScreenshot(Path.Combine(folderPath, saveName + ".png"));
    }

    private IEnumerator GrowFireRoutine()
    {
        while (_fireTransform.localScale.x < targetFireScale)
        {
            _fireTransform.localScale = Vector3.MoveTowards(
                _fireTransform.localScale, 
                Vector3.one * targetFireScale, 
                scaleGrowthSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private void FlipPlayer(float moveDir)
    {
        Vector3 scale = _playerTransform.localScale;
        scale.x = Mathf.Abs(scale.x) * (moveDir > 0 ? 1 : -1);
        _playerTransform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTransform = other.transform;
            _playerController = other.GetComponent<PlayerController>();
            _playerRb = other.GetComponent<Rigidbody2D>();
            _playerAnim = other.GetComponent<Animator>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerTransform = null;
    }
}