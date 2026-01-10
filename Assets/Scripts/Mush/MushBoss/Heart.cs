using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Heart : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int health = 60;
    private Animator anim;

    [Header("Scale Up")]
    [SerializeField] private bool hasGrowth;
    [SerializeField] private float startScale;
    [SerializeField] private float endScale;
    [SerializeField] private float scaleGrowth;
    [SerializeField] private float intervalTime;
    private float lastScaleUpTime = -Mathf.Infinity;

    [Header("Connected To Vines")]
    [SerializeField] private bool isConnectedToVines;
    [SerializeField] private GameObject vineObject;
    [SerializeField] private VineTouchSensor  vineTouchSensor;
    private SpriteRenderer[] vines;
    private SpriteRenderer heartSpriteRenderer;
    private Light2D heartLight2D;
    private Color transparentColor;
    private bool isBeingDestroyed = false;

    [Header("Connected To MushBoss")]
    [SerializeField] private bool isConnectedToMushBoss;
    private MushBoss boss;
    private SpriteRenderer bossRenderer;
    [SerializeField] private float colorReturnDuration = 0.83f;


    void Start()
    {
        anim = GetComponent<Animator>();

        if (hasGrowth)
        {
            transform.localScale = new Vector3(
                startScale * Mathf.Sign(transform.localScale.x),
                startScale * Mathf.Sign(transform.localScale.y),
                transform.localScale.z);
        }

        if (isConnectedToMushBoss)
        {
            boss = transform.parent.GetComponentInChildren<MushBoss>();
            if (boss != null)
                bossRenderer = boss.GetComponent<SpriteRenderer>();
        }

        if (isConnectedToVines)
        {
            heartSpriteRenderer =  GetComponent<SpriteRenderer>();
            heartLight2D = GetComponentInChildren<Light2D>();
            vines = vineObject.GetComponentsInChildren<SpriteRenderer>();
            vineTouchSensor = vineObject.transform.parent.GetComponentInChildren<VineTouchSensor>();
            
            Array.Reverse(vines);
            if (vines.Length > 0)
                transparentColor = new Color(vines[0].color.r, vines[0].color.g, vines[0].color.b, 0f);
        }
    }

    void Update()
    {
        if (health <= 0 && !isBeingDestroyed)
        {
            isBeingDestroyed = true;

            if (isConnectedToMushBoss && boss != null)
                boss.HeartWasDestroyed();

            if (isConnectedToVines && vines != null && vines.Length > 0)
            {
                vineTouchSensor.WhenDestroyed();
                heartSpriteRenderer.color = transparentColor;
                heartLight2D.intensity = 0;
                StartCoroutine(DestroyHeartAndVines());
                return;
            }

            Destroy(gameObject);
        }

        if (hasGrowth)
            ScaleUp();
    }

    private void ScaleUp()
    {
        if (Time.time - lastScaleUpTime >= intervalTime && Math.Abs(transform.localScale.x) < endScale)
        {
            lastScaleUpTime = Time.time;
            transform.localScale = new Vector3(
                transform.localScale.x + scaleGrowth * Mathf.Sign(transform.localScale.x),
                transform.localScale.y + scaleGrowth * Mathf.Sign(transform.localScale.y),
                transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Attack"))
        {
            PlayerController player = other.transform.parent.GetComponent<PlayerController>();
            if (player == null) return;

            int attackDamage = player.attackDamage;
            health -= attackDamage;

            if (isConnectedToMushBoss && boss != null)
            {
                boss.bossHealth -= attackDamage;
                if (bossRenderer != null)
                {
                    bossRenderer.color = Color.red;
                    boss.ReturnColor(colorReturnDuration);
                }
            }

            if (anim != null)
                anim.SetTrigger("damaged");
        }
    }

    // Coroutine to fade all vines and then destroy the heart
    private IEnumerator DestroyHeartAndVines()
    {
        if (vines != null)
        {
            foreach (var vine in vines)
            {
                if (vine != null)
                    yield return StartCoroutine(VineFade(0.45f, vine));
            }
        }

        Destroy(gameObject);
    }

    // Fade a single vine
    private IEnumerator VineFade(float duration, SpriteRenderer vine)
    {
        if (vine == null) yield break; // Safety check

        float t = 0f;
        Color startColor = vine.color;

        while (t < 1f)
        {
            if (vine == null) yield break; // Stop if destroyed elsewhere
            t += Time.deltaTime / duration;
            vine.color = Color.Lerp(startColor, transparentColor, t);
            yield return null;
        }

        if (vine != null)
        {
            vine.color = transparentColor;
            Destroy(vine.gameObject);
        }
    }
}
