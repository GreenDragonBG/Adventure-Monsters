using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HiddenPlant : ExtendedPlant
{
    [Header("Position")]
    private Vector3 attackPos;
    private Vector3 hiddenPos;

    [Header("Sprite Colors")]
    private SpriteRenderer[] spriteRenderers;
    private List<Color> spriteColors;
    private readonly Color transparent = new Color(0, 0, 0, 0);

    [Header("Lighting")] 
    private new Light2D light;

    protected override void Start()
    {
        base.Start();

        attackPos = transform.position;
        hiddenPos = new Vector3(attackPos.x, attackPos.y - 0.65f, attackPos.z);
        transform.position = hiddenPos;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        spriteColors = new List<Color>();

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            spriteColors.Add(sr.color);
            sr.color = transparent;
        }
        
        light = gameObject.GetComponentInChildren<Light2D>();
        light.intensity = 0;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        { 
            if (CanAttack)
            {
                playerAnimator = other.GetComponent<Animator>();

                MakeVisible();

                StartCoroutine(MoveToAttack(4f));
                StartCoroutine(Attack()); 
            }
        }
    }

    // ========== Movement + Visibility ==========

    private IEnumerator MoveToAttack(float speed)
    {
        while (transform.position.y < attackPos.y)
        {
            float newY = Mathf.MoveTowards(transform.position.y, attackPos.y, speed * Time.deltaTime);
            transform.position = new Vector3(attackPos.x, newY, attackPos.z);
            yield return null;
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
        CanAttack = true;
    }

    private void MakeVisible()
    {
        light.intensity = 1;
        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].color = spriteColors[i];
    }

    private void MakeInvisible()
    {
        light.intensity = 0;
        foreach (var sr in spriteRenderers)
            sr.color = transparent;
    }
}
