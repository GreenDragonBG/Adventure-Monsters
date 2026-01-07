using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class Chomper : MonoBehaviour
{
    [Header("Animation Variables")]
    private static readonly int LaunchSeed = Animator.StringToHash("launchSeed");
    private static readonly int WaveScream = Animator.StringToHash("waveScream");
    private static readonly int IsStunned = Animator.StringToHash("isStunned");
    private static readonly int IsCharging = Animator.StringToHash("isCharging");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int LeftChomp = Animator.StringToHash("leftChomp");
    private static readonly int RightChomp = Animator.StringToHash("rightChomp");
    private static readonly int DamageTaken = Animator.StringToHash("DamageTaken");

    [Header("References")]
    [SerializeField] private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    
    [Header("BossHealth")]
    [SerializeField ]private GameObject bossBar;
    private BossBar bossBarScript;
    private int bossHealth = 1680;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDistance = 0.4f;
    [SerializeField] private float slowDistance = 1f;
    [SerializeField] private float viewRange = 5f;
    private float previousScaleX;

    [Header("Roaming")]
    [SerializeField] private float roamTimeMin = 1f;
    [SerializeField] private float roamTimeMax = 3f;
    private bool isRoaming;
    private int roamDirection = 1;

    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkDistance = 0.15f;
    private bool groundAhead;
    private bool wallAhead;

    [Header("Charge Attack")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 2f;
    [SerializeField] private float stunDuration = 1.5f;
    private bool isCharging;
    private bool isStunned;
    private int chargeDirection;
    [SerializeField] private float retreatDuration = 2f;
    private bool isRetreating;
    private int retreatDirection;

    [Header("Chomp Attack")] 
    private bool playerInsideChompRange;

    [Header("Attacks")] 
    [SerializeField] private GameObject attacksParent;
    [SerializeField] private float closeDistance = 1.5f;
    [SerializeField] private float midDistance = 7f;
    [SerializeField] private float longDistance = 15f;
    private SeedLauncher seedLauncher;
    private ChompWave chompWave;
    private CameraShake camShake;

    [Header("Attack Timer")]
    private float attackTimer;
    [SerializeField] private float attackDelay = 2f;
    private float activeAttackDelay;
    private bool isAttacking;
    private bool takenDamage;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        camShake = Camera.main?.GetComponent<CameraShake>();
        seedLauncher = attacksParent.GetComponentInChildren<SeedLauncher>();
        chompWave = attacksParent.GetComponentInChildren<ChompWave>();
        
        //gets the boss bar script and sets  health
        bossBar.SetActive(true);
        bossBarScript = bossBar.GetComponent<BossBar>();
        bossBarScript.maxHealth = bossHealth;
        bossBarScript.currentHealth = bossHealth;

        activeAttackDelay = attackDelay;
        
        previousScaleX = transform.localScale.x;
        StartCoroutine(RoamRoutine());
    }

    private void FixedUpdate()
    {
        HealthChange();
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        
        if (!state.IsName("Hurt"))
        {
            takenDamage = false;
        }

        if (!player) return;

        UpdateChecks();

        if (isStunned)//While stunned it cant move
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
        
        if (isRetreating)
        {
            rb.linearVelocity = new Vector2(retreatDirection * speed, rb.linearVelocity.y);
            return;
        }


        if (isCharging)//Charge moveing
        {
            ChargeMove();
            return;
        }

        // Stop movement during attack animations (except charge)
        if (state.IsTag("Attack"))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            isAttacking = true;
            attackTimer = 0f;
            return;
        }
        else
        {
            isAttacking = false;
        }

        // Normal movement only if not attacking
        if (!isAttacking)
            Move();

        // Trigger attack after attackDelay and Non-Attack animations
        if ((!state.IsTag("Attack") && !isAttacking))
        {
            attackTimer += Time.fixedDeltaTime;
            float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
            if (distanceToPlayer < closeDistance && attackTimer>=0.3f)
            {
                FacePlayer();
                isAttacking = true;
                int result = Random.Range(0, 2);
                anim.SetTrigger(result == 0 ? LeftChomp : RightChomp);
            }
            else if (attackTimer >= activeAttackDelay)
            {
                CalcAttack(distanceToPlayer);
                attackTimer = 0f;
            }
        }
        else
        {
            attackTimer = 0f;
        }
    }
        
    private void HealthChange()
    {
        if (bossBarScript.currentHealth<=0)
        {
            bossBar.SetActive(false);
             anim.SetTrigger("Death");
            
             enabled = false;
        }
        else
        {
            bossBarScript.currentHealth = bossHealth;
        }
    }
    
    private void UpdateChecks()//Checks if there is a wall or ground
    {
        groundAhead = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            checkDistance,
            groundLayer
        );

        wallAhead = Physics2D.Raycast(
            wallCheck.position,
            transform.right,
            checkDistance,
            groundLayer
        );
    }

    private void Move()//if not attacking go after player if close enough or roam freely searching for player
    {
        if (isAttacking || takenDamage) return;

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        if (distanceToPlayer <= viewRange)
        {
            // PLAYER IN VIEW → CHASE
            isRoaming = false;

            float direction = Mathf.Sign(player.position.x - transform.position.x);
            FlipWithOffset(direction);

            if (distanceToPlayer <= stopDistance || wallAhead || !groundAhead)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                anim.SetBool(IsWalking, false);
                return;
            }

            float moveSpeed = speed;
            if (distanceToPlayer < slowDistance)
                moveSpeed *= distanceToPlayer / slowDistance;

            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            anim.SetBool(IsWalking, true);
        }
        else
        {
            // PLAYER OUT OF VIEW → ROAM
            isRoaming = true;
        }
    }

    private void RoamMove()//the roams movement
    {
        if (isAttacking || takenDamage) return;

        if (wallAhead || !groundAhead)
            roamDirection *= -1;

        rb.linearVelocity = new Vector2(roamDirection * speed, rb.linearVelocity.y);
        FlipWithOffset(roamDirection);
        anim.SetBool(IsWalking, true);
    }

    private IEnumerator RoamRoutine()//randomly goes around searching for the player
    {
        while (true)
        {
            if (isRoaming)
            {
                roamDirection = Random.value < 0.5f ? -1 : 1;
                float roamDuration = Random.Range(roamTimeMin, roamTimeMax);
                float elapsed = 0f;

                while (elapsed < roamDuration && isRoaming)
                {
                    RoamMove();
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator Charge()//flips towards the player and set the charge animation and trigger the movement
    {
        if (isCharging || isStunned)
            yield break;
        
        chargeDirection = (int)Mathf.Sign(player.position.x - transform.position.x);
        FlipWithOffset(chargeDirection);

        isCharging = true;

        chargeDirection = (int)Mathf.Sign(player.position.x - transform.position.x);
        FlipWithOffset(chargeDirection);

        anim.SetBool(IsCharging, true);

        float startTime = Time.time;

        while (Time.time - startTime < chargeDuration)
            yield return new WaitForFixedUpdate();

        isCharging = false;
        anim.SetBool(IsCharging, false);
    }

    private void ChargeMove()//moving during charge attack
    {
        rb.linearVelocity = new Vector2(chargeDirection * chargeSpeed, rb.linearVelocity.y);

        if (wallAhead && !isStunned)
            StartCoroutine(Stun());
    }

    private IEnumerator Stun()//Play stun animation and make the chomper unable to move if he hits a wall during a charge
    {
        isCharging = false;
        isStunned = true;

        anim.SetBool(IsStunned, true);
        anim.SetBool(IsCharging, false);

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration);

        anim.SetBool(IsStunned, false);
        isStunned = false;
    }

    private IEnumerator RetreatFromPlayer()//After charge if hit player step back to give player space
    {
        isCharging = false;
        isAttacking = false;
        // Move AWAY from player
        retreatDirection = -(int)Mathf.Sign(player.position.x - transform.position.x);
        FlipWithOffset(retreatDirection);
        isRetreating = true;
        
        float elapsed = 0f;

        while (elapsed < retreatDuration)
        {
            rb.linearVelocity = new Vector2(retreatDirection * speed, rb.linearVelocity.y);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isRetreating = false;
    }

    private void FlipWithOffset(float direction)//flip the chomper but with offset as its asset is not centered
    {
        if (isCharging || isRetreating) return;

        float newScaleX = -Mathf.Abs(transform.localScale.x) * direction;

        if (previousScaleX > 0 && newScaleX < 0)
            transform.position += new Vector3(1.12f, 0f, 0f);
        else if (previousScaleX < 0 && newScaleX > 0)
            transform.position += new Vector3(-1.12f, 0f, 0f);

        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        previousScaleX = newScaleX;
    }
    
    private void FacePlayer(bool allowDuringAttack = true)
    {
        if (!player || isCharging || isStunned || (!allowDuringAttack && isAttacking)) return;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        FlipWithOffset(direction);

        
        if (attacksParent != null)//Make the attacksParent face the same direction and be at the same place as the Chomper
        {
            Vector3 parentScale = attacksParent.transform.localScale;
            parentScale.x = Mathf.Abs(parentScale.x) * Mathf.Sign(transform.localScale.x);
            attacksParent.transform.localScale = parentScale;
            
            attacksParent.transform.position = transform.position;
        }
    }


//Called by animations
    public void SeedAttack()
    {
        seedLauncher?.LaunchSeed();
    }

    public void WaveAttack()
    {
        chompWave?.WaveAttack();
    }

    public void ScreamShake()
    {
        if (camShake != null)
            StartCoroutine(camShake.Shake(0.6f, 0.2f));
    }

    public void ChompDamageCheck()
    {
        if (playerInsideChompRange)
        {
            player.GetComponent<Animator>()?.SetTrigger("Damage");
            DoDamage.DealDamage();
        }
    }

    private void CalcAttack(float distanceToPlayer) //The logic behind what attack gets used (based on distance and chance)
    {
        int randResult = Random.Range(0, 10);
        
        activeAttackDelay = attackDelay;//sets to normal delay
        
        if (distanceToPlayer < midDistance)
        {
            if (randResult < 5)
            {
                FacePlayer();
                if (attackDelay<4f)//long enough to fully play
                {
                    activeAttackDelay = 4f;
                }
                isAttacking = true;
                anim.SetTrigger(WaveScream);
            }
            else if (randResult < 8)
            {
                FacePlayer();
                isAttacking = true;
                anim.SetTrigger(LaunchSeed);
            }
        }
        else if (distanceToPlayer < longDistance)
        {
            if (randResult < 3)
            {
                FacePlayer();
                isAttacking = true;
                anim.SetTrigger(LaunchSeed);
            }
            else if (randResult < 5)
            {
                FacePlayer();
                if (attackDelay<4f)//long enough to fully play
                {
                    activeAttackDelay = 4f;
                }
                isAttacking = true;
                anim.SetTrigger(WaveScream);
            }
            else if (randResult < 6)
            {
                StartCoroutine(Charge());
            }
        }
        else
        {
            if (randResult < 7)
            {
                StartCoroutine(Charge());
            }
        }

    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isCharging && other.gameObject.CompareTag("Player"))
        {
            isCharging = false;
            anim.SetBool(IsCharging, false);

            player.gameObject.GetComponent<Animator>()?.SetTrigger("Damage");
            DoDamage.DealDamage();
            
            StartCoroutine(RetreatFromPlayer());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Attack") && !isAttacking)
        {
            bossHealth -= 30;
            anim.SetTrigger(DamageTaken);
            takenDamage = true;
        } 
        else if (other.CompareTag("Player"))
        {
            playerInsideChompRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideChompRange = false;
        }
    }

    private void HasDied()
    {
        anim.SetBool("isDeath",true);
        rb.bodyType = RigidbodyType2D.Static;
        foreach (Collider2D coll in GetComponentsInChildren<Collider2D>())
        {
            coll.enabled = false;
        }
    }
}
