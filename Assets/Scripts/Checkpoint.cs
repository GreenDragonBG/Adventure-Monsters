using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Checkpoint : MonoBehaviour
{
    private static Transform playerSpawnPoint;
    private static GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerSpawnPoint = player.gameObject.transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerSpawnPoint= this.gameObject.transform;
        }
    }

    public static void Respawn()
    {
        player.transform.position = playerSpawnPoint.position;
    }
}
