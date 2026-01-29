using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    public static CameraController Instance;
    public GameObject target;
    public float smoothValue = 2;
    public float posY = 1;

    public static Vector3 SpawnPos = Vector3.zero; // Use this for EVERYTHING

    private void Awake()
    {
        // 1. Check our unified static variable
        if (SpawnPos != Vector3.zero)
        {
            StartCoroutine(TeleportAndSyncParallax(SpawnPos));
            
            // CRITICAL: Reset it to zero immediately so the NEXT scene 
            // doesn't accidentally use this same position!
            SpawnPos = Vector3.zero; 
        }
        // 2. Only load save if we didn't have a transition spawn
        else if (PlayerController.ShouldTeleportToSave && SaveSystem.CurrentData != null && !SaveSystem.CurrentData.isNewGame)
        {
            StartCoroutine(TeleportAndSyncParallax(SaveSystem.CurrentData.cameraPos));
        }
    }

    private IEnumerator TeleportAndSyncParallax(Vector3 destination)
    {
        yield return new WaitForEndOfFrame();
        transform.position = destination;
    }

    void FixedUpdate()
    {
        if (target == null) return;
        Vector3 targetpos = new Vector3(target.transform.position.x, target.transform.position.y + posY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetpos, Time.deltaTime * smoothValue);
    } 
}