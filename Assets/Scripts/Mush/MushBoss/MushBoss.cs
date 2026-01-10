using System;
using System.Collections;
using Mush;
using UnityEngine;
using Random = UnityEngine.Random;

public class MushBoss : MonoBehaviour
{
    [Header("BossHealth")]
    [SerializeField ]private GameObject bossBar;
    private Animator anim;
    private BossBar bossBarScript;
    public int bossHealth = 120;
    private Coroutine colorCoroutine;
    private SpriteRenderer spriteRenderer;
    private Color defaultColor;
    
    [Header("Heart")]
    private GameObject heartObject;
    [SerializeField]private GameObject heartPrefab;
    [SerializeField]private float heartYAxis;
    [SerializeField]private float heartMinSpawnX;
    [SerializeField]private float heartMaxSpawnX;
    [SerializeField]private float heartMaxCooldown;
    private float heartTimeDestroyed = -Mathf.Infinity;
    private float heartCurrentCooldown;
    private bool heartWasDestroyed = true;
    
    //Orb vars
    [Header("Orbs")]
    [SerializeField] public float attackInterval= 6;
    private float timeSinceAttack;
    public bool isAttacking;
    private MushOrb[] orbs;
    
    //Size changing on orb attack vars
    [Header("Orbs animation size change")]
    [SerializeField] private float sizeChangerValue;
    private float attackSize;
    private float normalSize;
    
    //Spore vars
    [Header("Spores")]
    [SerializeField] public float sporeInterval = 4.3f;
    private MushSpores spores;
    private float lastSporeCalculated;
    
    [Header("Head Hit")]
    [SerializeField] private HeadHit headHit;

    void Start()
    {
        //sets the orbs and spores game objects
        spores = transform.parent.GetComponentInChildren<MushSpores>();
        orbs = transform.parent.GetComponentsInChildren<MushOrb>(true);
        //sets sizes on attack
        attackSize = 1.2f;
        normalSize = 1f;
        
        //sets the time of last orb attack and last spore calcualted on that Start 
        timeSinceAttack = Time.time;
        lastSporeCalculated = Time.time;
        
        //gets the boss bar script and sets  health
        bossBar.SetActive(true);
        bossBarScript = bossBar.GetComponent<BossBar>();
        bossBarScript.maxHealth = bossHealth;
        bossBarScript.currentHealth = bossHealth;
        
        //sets the attack time for headHit
        headHit.enabled = true;
        
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    //Constantly keeps track of the size and attacks on every time intreval
    private void Update()
    {
        HeartSpawner();
        
        ChangeCanHeadHit();        

        HealthChange();
        
        ChangeSize();
        
        if (((Time.time - timeSinceAttack) >= attackInterval) && !isAttacking)
        {
            Attack();
        }
        
        if ((Time.time - lastSporeCalculated) >= sporeInterval)
        {
            SporeCalculate();
        }
    }
    
    //on attack grows in size and get back to normal size
    private void ChangeSize()
    {
        if (isAttacking)
        {
            transform.localScale = new Vector3(1, transform.localScale.y + sizeChangerValue, 1);
            if (transform.localScale.y >= attackSize)
            {
                isAttacking = false;
            }
        }
        else if ( transform.localScale.y > normalSize)
        {
            transform.localScale = new Vector3(1,transform.localScale.y - 0.003f, 1);
        }
    }

    private void HealthChange()
    {
        if (bossBarScript.currentHealth<=0)
        {
            bossBar.SetActive(false);
            headHit.enabled = false;
            foreach (MushOrb orb in orbs)
            {
                orb.enabled = false;
            }
            if (heartObject!=null)
            {
                Destroy(heartObject);
            }
            enabled = false;
        }
        else
        {
            bossBarScript.currentHealth = bossHealth;
        }
    }

    //sends out the orbs and calculates if it has to send the spores too
    private void Attack()
    {
        timeSinceAttack = Time.time;
        LaunchAll(orbs);
    }

    private void SporeCalculate()
    {
        if (!orbs[1].toLaunch)
        {
            spores.CalculateChance();
        }
        lastSporeCalculated = Time.time;
    }
    
    private void LaunchAll(MushOrb[] orbs)
    {
        if (!spores.isAttacking && orbs[^1].enabled)
        {
            //for each orb that is called it gets activated and launched
            foreach (MushOrb orb in orbs)
            {
                orb.gameObject.SetActive(true);
                orb.StartLaunch(this);
            }
        }
    }

    private void ChangeCanHeadHit()
    {
        if (spores.isAttacking)
        {
            headHit.canAttack = false;
        }
        else if(!headHit.canAttack)
        {
            headHit.canAttack = true;
            headHit.lastAttackTime = Time.time;
        }
    }

    private void HeartSpawner()
    {
        if (heartWasDestroyed)
        {
            heartTimeDestroyed =  Time.time;
            heartCurrentCooldown = Random.Range(0, heartMaxCooldown);
            
            heartWasDestroyed = false;
            
        }else if (!float.IsNegativeInfinity(heartTimeDestroyed) && Time.time - heartTimeDestroyed >= heartCurrentCooldown)
        {
            Vector2 newPosition = new Vector2(Random.Range(heartMinSpawnX,heartMaxSpawnX),heartYAxis);
            heartObject =Instantiate(heartPrefab, newPosition, Quaternion.identity);
            heartObject.transform.parent = transform.parent;
            
            heartTimeDestroyed = -Mathf.Infinity;
        }
        
    }
    
    private IEnumerator ReturnColorOverTime(float duration)
    {
        float t = 0f;
        Color startColor = spriteRenderer.color;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            spriteRenderer.color = Color.Lerp(startColor, defaultColor, t);
            yield return null;
        }

        spriteRenderer.color = defaultColor;
    }

    public void ReturnColor(float duration)
    {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(ReturnColorOverTime(duration));
    }

    public void HeartWasDestroyed()
    {
        heartWasDestroyed = true;
    }
}