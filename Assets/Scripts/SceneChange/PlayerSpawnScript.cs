using UnityEngine;

public class PlayerSpawnScript : MonoBehaviour
{
    public static Vector3 SpawnPos;
    public static Vector3 CameraSpawnPos;
    private Camera mainCamera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (!SpawnPos.Equals(Vector3.zero))
        {
            transform.position = SpawnPos;
        }

        if (!CameraSpawnPos.Equals(Vector3.zero))
        {
            mainCamera = Camera.main;
            mainCamera.transform.position = CameraSpawnPos;
        }
    }
}
