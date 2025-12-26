using UnityEngine;
using System.Collections;

public class Chomper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

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
    [SerializeField] private float chargeMinDistance = 2.5f;
    private bool isCharging;
    private bool isStunned;
    private int chargeDirection;

    [Header("Attacks")] 
    [SerializeField] private GameObject attacksParent;
    [SerializeField] private float closeDistance = 1.5f;
    [SerializeField] private float midDistance = 7f;
    [SerializeField] private float longDistance = 15f;
    private SeedLauncher seedLauncher;
    private ChompWave chompWave;
    private CameraShake camShake;

    [Header("Attack Timer")]
    private float attackTimer = 0f;
    [SerializeField] private float attackDelay = 2f;
    private bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        camShake = Camera.main?.GetComponent<CameraShake>();
        seedLauncher = attacksParent.GetComponentInChildren<SeedLauncher>();
        chompWave = attacksParent.GetComponentInChildren<ChompWave>();

        previousScaleX = transform.localScale.x;
        StartCoroutine(RoamRoutine());
    }

    private void FixedUpdate()
    {
        if (!player) return;

        UpdateChecks();

        if (isStunned)//While stunned it cant move
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isCharging)//Charge moveing
        {
            ChargeMove();
            return;
        }

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

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

        // Trigger attack after attackDelay and Idle/Walk animation
        if ((state.IsName("Idle") || state.IsName("Walk")) && !isAttacking)
        {
            attackTimer += Time.fixedDeltaTime;
            if (attackTimer >= attackDelay)
            {
                CalcAttack();
                attackTimer = 0f;
            }
        }
        else
        {
            attackTimer = 0f;
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
        if (isAttacking) return;

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
                anim.SetBool("isWalking", false);
                return;
            }

            float moveSpeed = speed;
            if (distanceToPlayer < slowDistance)
                moveSpeed *= distanceToPlayer / slowDistance;

            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            anim.SetBool("isWalking", true);
        }
        else
        {
            // PLAYER OUT OF VIEW → ROAM
            isRoaming = true;
        }
    }

    private void RoamMove()//the roams movement
    {
        if (isAttacking) return;

        if (wallAhead || !groundAhead)
            roamDirection *= -1;

        rb.linearVelocity = new Vector2(roamDirection * speed, rb.linearVelocity.y);
        FlipWithOffset(roamDirection);
        anim.SetBool("isWalking", true);
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

        anim.SetBool("isCharging", true);

        float startTime = Time.time;

        while (Time.time - startTime < chargeDuration)
            yield return new WaitForFixedUpdate();

        isCharging = false;
        anim.SetBool("isCharging", false);
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

        anim.SetBool("isStunned", true);
        anim.SetBool("isCharging", false);

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration);

        anim.SetBool("isStunned", false);
        isStunned = false;
    }

    private void FlipWithOffset(float direction)//flip the chomper but with offset as its asset is not centered
    {
        if (isCharging) return;

        float newScaleX = -Mathf.Abs(transform.localScale.x) * direction;

        if (previousScaleX > 0 && newScaleX < 0)
            transform.position += new Vector3(1.12f, 0f, 0f);
        else if (previousScaleX < 0 && newScaleX > 0)
            transform.position += new Vector3(-1.12f, 0f, 0f);

        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        previousScaleX = newScaleX;
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

    private void CalcAttack() //The logic behind what attack gets used (based on distance and chance)
    {
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        int randResult = Random.Range(0, 10);

        if (distanceToPlayer < closeDistance)
        {
            FacePlayer();
            isAttacking = true;
            anim.SetTrigger("chomp");
        }
        else if (distanceToPlayer < midDistance)
        {
            if (randResult < 5)
            {
                FacePlayer();
                isAttacking = true;
                anim.SetTrigger("launchSeed");
            }
            else if (randResult < 8)
            {
                FacePlayer();
                isAttacking = true;
                anim.SetTrigger("waveScream");
            }
        }
        else if (distanceToPlayer < longDistance)
        {
            if (randResult < 3)
            {
                FacePlayer();
                isAttacking = true;
                anim.SetTrigger("waveScream");
            }
            else if (randResult < 5)
            {
                FacePlayer();
                isAttacking = true;
                anim.SetTrigger("launchSeed");
            }
            else if (randResult < 6)
            {
                StartCoroutine(Charge());
            }
        }
        else
        {
            if (randResult < 6)
            {
                StartCoroutine(Charge());
            }
        }

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


}
