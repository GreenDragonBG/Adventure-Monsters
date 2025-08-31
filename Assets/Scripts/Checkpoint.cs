using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Checkpoint : MonoBehaviour
{
    private static Transform playerSpawnPoint;
    private static GameObject player;
    private static Animator animator;
    private static Rigidbody2D rb2d;
    private static bool waitingForAnimation = false;
    private static float animationDelay = 0.2f;
    private static float timeStartedWaiting = 0f;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerSpawnPoint = player.gameObject.transform;
        animator = player.GetComponent<Animator>();
        rb2d = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Respawn"))
        {
            player.transform.position = playerSpawnPoint.position;
            animator.SetBool("Teleport", false);
            waitingForAnimation = true;
            timeStartedWaiting = Time.time;
        }

        if (waitingForAnimation && Time.time- timeStartedWaiting>animationDelay)
        {
            rb2d.constraints=RigidbodyConstraints2D.FreezeRotation;
            waitingForAnimation = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerSpawnPoint!=this.gameObject.transform)
        {
            playerSpawnPoint= this.gameObject.transform;
        }
    }

    public static void Respawn()
    {
        rb2d.constraints=RigidbodyConstraints2D.FreezeAll;
        animator.SetBool("Teleport", true);
        animator.SetTrigger("Damage");
    }
}
