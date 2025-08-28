using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class CameraController : MonoBehaviour {
    public static CameraController Instance;
    [FormerlySerializedAs("Target")] public GameObject target;
    [FormerlySerializedAs("Smoothvalue")] public int smoothvalue =2;
    [FormerlySerializedAs("PosY")] public float posY = 1;
    
    // Use this for initialization
    public Coroutine MyCo;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 targetpos = new Vector3(target.transform.position.x, target.transform.position.y + posY, -100);
        transform.position = Vector3.Lerp(transform.position, targetpos, Time.deltaTime * smoothvalue);
    } 
}