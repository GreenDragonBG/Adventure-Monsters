using UnityEngine;

public class PlayerSpawnScript : MonoBehaviour
{
    public static Vector3 SpawnPos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!SpawnPos.Equals(Vector3.zero))
        {
            transform.position = SpawnPos;
        }
    }
}
