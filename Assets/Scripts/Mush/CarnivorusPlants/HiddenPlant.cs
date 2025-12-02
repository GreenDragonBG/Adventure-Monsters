using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPlant : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField]private bool canDoDamage = true;
    private bool canAttack = true;
        
    [Header("Animation")]
    private Animator animator;
    private Animator playerAnimator;
    
    [Header("Position")]
    private Vector3 attackPos;
    private Vector3 hiddenPos;
    
    [Header("Sprite Colors")]
    private SpriteRenderer[] spriteRenderers;
    private List<Color> spriteColors;
    void Start()
    {
        animator = GetComponent<Animator>();
        
        attackPos = transform.position;
        hiddenPos = new Vector3(attackPos.x, attackPos.y - 0.65f, attackPos.z);
        transform.position = hiddenPos;
        
        spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        spriteColors = new List<Color>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteColors.Add(spriteRenderer.color);
            spriteRenderer.color = new Color(0, 0, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            playerAnimator = other.GetComponent<Animator>();
            MakeVisible();
            StartCoroutine(MoveToAttack(4f));
            StartCoroutine(Attack());
        }
    }
    
    private IEnumerator Attack()
    {
        animator.SetTrigger("Attack");
        canAttack = false;
        yield break;
    }
    
    private IEnumerator MoveToAttack(float speed)
    {
        while (transform.position.y < attackPos.y)
        {
            float newY = Mathf.MoveTowards(transform.position.y, attackPos.y, speed * Time.deltaTime);
            transform.position = new Vector3(attackPos.x, newY, attackPos.z);
            
            yield return null;
        }
        
    }

    private void MakeVisible()
    {
        for (int i =0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = spriteColors[i];
        }
    }

    private IEnumerator MoveToHiding(float speed)
    {
        while (transform.position.y > hiddenPos.y)
        {
            float newY = Mathf.MoveTowards(transform.position.y, hiddenPos.y, speed * Time.deltaTime);
            transform.position = new Vector3(hiddenPos.x, newY, hiddenPos.z);
            
            yield return null;
        }

        MakeInvisible();
        canAttack = true;
    }
    
    private void MakeInvisible()
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = new Color(0, 0, 0, 0);
        }
    }

    private void DealTheDamage()
    {
        if (canDoDamage)
        {
            playerAnimator.SetTrigger("Damage");
            DoDamage.DealDamage();
        }
    }
}
