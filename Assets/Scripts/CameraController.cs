using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    public static CameraController Instance;
    public GameObject target;
    public float smoothValue = 2;
    public float posY = 1;

    public static Vector3 SpawnPos;
    
    private void Awake()
    {
        if (PlayerController.ShouldTeleportToSave && SaveSystem.CurrentData != null && !SaveSystem.CurrentData.isNewGame)
        {
            StartCoroutine(TeleportAndSyncParallax(SaveSystem.CurrentData.cameraPos));
        }else if (SpawnPos != Vector3.zero)
        {
            StartCoroutine(TeleportAndSyncParallax(SpawnPos));
            SpawnPos = Vector3.zero; 
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