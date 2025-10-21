using System;
using Mush;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MushBoss : MonoBehaviour
{
    [Header("BossHealth")]
    [SerializeField ]private GameObject bossBar;
    private BossBar bossBarScript;
    private float bossHealth = 80f;
    
    
    
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
        bossBarScript.maxHealth = 100f;
        bossBarScript.currentHealth = bossHealth;
        
        //sets the attack time for headHit
        headHit.enabled = true;
    }

    //Constantly keeps track of the size and attacks on every time intreval
    private void Update()
    {
        changeCanHeadHit();        

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

    private void changeCanHeadHit()
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
}