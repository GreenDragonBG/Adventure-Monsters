using System;
using System.Collections;
using UnityEngine;

public class Thorne : MonoBehaviour
{
    [SerializeField] private float minDistanceBetween;
    
    [SerializeField] private float timeItLasts;
    private float timeSpawned;
    
    private Animator animator;
    private bool isShrinking = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        timeSpawned = Time.time;
    }

    private void Update()
    {
        if (!isShrinking && Time.time-timeSpawned >= timeItLasts)
        {
            StartCoroutine(ShrinkAndDestroy());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") //damage player
        {
            DoDamage.DealDamage();
            other.GetComponent<Animator>().SetTrigger("Damage");
        } 
        else if (other.GetComponent<Thorne>()!=null) // destroy itself if it overlaps another one 
        {
            if ((other.transform.position.x > transform.position.x && (other.transform.position.x- transform.position.x)<minDistanceBetween)
                ||
                (other.transform.position.x < transform.position.x && (transform.position.x-other.transform.position.x)<minDistanceBetween))
            {
                //Destroys the oldest thorne
                Destroy((other.GetComponent<Thorne>().timeSpawned<timeSpawned) ? other.gameObject : gameObject);
            }
        }
    }
    
    private IEnumerator ShrinkAndDestroy()
    {
        isShrinking = true;
        animator.SetTrigger("shirnk");

        // Wait for current animation state length
        yield return new WaitForSeconds(
            animator.GetCurrentAnimatorStateInfo(0).length
        );

        Destroy(gameObject);
    }}
