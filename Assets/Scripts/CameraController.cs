using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {
    public static CameraController Instance;
    [FormerlySerializedAs("Target")] public GameObject target;
    [FormerlySerializedAs("Smooth Value")] public float smoothValue = 2;
    [FormerlySerializedAs("PosY")] public float posY = 1;
    
    public Coroutine MyCo;

    public static Vector3 SpawnPos;
    
    private void Awake()
    {
        // 1. Capture the editor position to calculate the parallax delta
        float initialX = transform.position.x;

        // 2. Handle Scene Transitions (AreaExit logic)
        if (!SpawnPos.Equals(Vector3.zero))
        {
            transform.position = SpawnPos;
            UpdateParallaxOnTeleport(initialX, SpawnPos.x);
            SpawnPos = Vector3.zero; 
        }
        // 3. Handle Save Loads
        else if (SaveSystem.CurrentData != null && !SaveSystem.CurrentData.isNewGame && SaveSystem.CurrentData.lastScene == SceneManager.GetActiveScene().name)
        {
            Vector3 savedPos = SaveSystem.CurrentData.cameraPos;
            transform.position = savedPos;
            UpdateParallaxOnTeleport(initialX, savedPos.x);
        }
    }

    private void UpdateParallaxOnTeleport(float oldX, float newX)
    {
        ParallaxCamera parallax = GetComponent<ParallaxCamera>();
        if (parallax != null)
        {
            float delta = oldX - newX;
            parallax.ForceResetBackground(delta);
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;
        Vector3 targetpos = new Vector3(target.transform.position.x, target.transform.position.y + posY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetpos, Time.deltaTime * smoothValue);
    } 
}