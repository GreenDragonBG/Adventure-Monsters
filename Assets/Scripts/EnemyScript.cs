using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyScript : MonoBehaviour
{
    public int health = 100;
    public static bool PlayerIsAttacked= false;
    private Animator animator;
    private float speed = 2f; // Movement speed
    public int direction = -1; // Start moving left
    public bool attacking = false;
    [FormerlySerializedAs("LastTimeHitAWall")] public float lastTimeHitAWall = -Mathf.Infinity;
    [FormerlySerializedAs("LastTimeHitAPlayer")] public float lastTimeHitAPlayer = -Mathf.Infinity;
    [FormerlySerializedAs("TimeFromDead")] public float timeFromDead = -Mathf.Infinity;
    private bool haveToTurn = false;
    public bool hitFromBihind;
    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", true);
    }

    private void Update()
    {
        // Move the enemy in the current direction if not attacking
        //And Notify that your not Attacking
        if (!attacking && Time.time-lastTimeHitAPlayer>0.6 && health > 0 )
        {
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
            if (haveToTurn)
            {
                haveToTurn = false;
                ReverseDirection();
            }
            PlayerIsAttacked = false;
        }
        else if (attacking)
        {
            // if attacking trigger animation and start cooldown
            //And Notify that your Attacking
            PlayerIsAttacked = true;
            lastTimeHitAPlayer = Time.time;
            animator.SetTrigger("attack");
            attacking = false;
        }

        if (health <= 0  && Time.time-timeFromDead>1 &&  Time.time-lastTimeHitAPlayer>0.7)
        {
            Destroy(gameObject);
        }
    }
    
    public void ReverseDirection()
    {
        //if attacking remeber to rotate
        if (attacking)
        {
            haveToTurn = true;
        }
        else
        {
            //if not attacking rotate
            direction *= -1;
            Vector3 newScale = gameObject.transform.localScale;
            newScale.x *= -1;
            gameObject.transform.localScale = newScale;
        }
    }
}
