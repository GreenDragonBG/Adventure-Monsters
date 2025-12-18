using System;
using System.Collections;
using UnityEngine;

public class ExtendedPlant : MonoBehaviour
{
    [Header("Attack Settings")]
    protected bool CanAttack = true;
    private static bool _canDamage = true;
    private const float DamageCooldown = 0.2f;
    private static float _lastTimeDamaged = -Mathf.Infinity;
        
    [Header("Animation")]
    private Animator animator;
    protected Animator PlayerAnimator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(_canDamage) return;
        
        if (float.IsNegativeInfinity(_lastTimeDamaged))
        {
            _lastTimeDamaged = Time.time;
        }
        else if(Time.time - _lastTimeDamaged >= DamageCooldown)
        {
            _lastTimeDamaged = -Mathf.Infinity;
            _canDamage = true;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            if (CanAttack)
            {
                PlayerAnimator = other.GetComponent<Animator>();
                StartCoroutine(Attack());
                CanAttack = true;
            }
        }
    }

    public IEnumerator Attack()
    {
        animator.SetTrigger("Attack");
        CanAttack = false;
        yield break;
    }
    
    protected void DealTheDamage()
    {
        if (_canDamage)
        {
            PlayerAnimator.SetTrigger("Damage");
            DoDamage.DealDamage();
            _canDamage = false;
        }
    }
}