using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class CameraController : MonoBehaviour {
    public static CameraController Instance;
    [FormerlySerializedAs("Target")] public GameObject target;
    [FormerlySerializedAs("Smooth Value")] public float smoothValue = 2;
    [FormerlySerializedAs("PosY")] public float posY = 1;
    
    public Coroutine MyCo;

    public static Vector3 SpawnPos;
    
    private void Awake()
    {
        if (!SpawnPos.Equals(Vector3.zero))
        {
            transform.localPosition = SpawnPos;
        }
    }
    void FixedUpdate()
    {
        Vector3 targetpos = new Vector3(target.transform.position.x, target.transform.position.y + posY, -100);
        transform.position = Vector3.Lerp(transform.position, targetpos, Time.deltaTime * smoothValue);
    } 
}