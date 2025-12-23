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

    [Header("Roaming")]
    [SerializeField] private float roamTimeMin = 1f;
    [SerializeField] private float roamTimeMax = 3f;

    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkDistance = 0.15f;

    [Header("Charge Attack")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 2f;
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private float chargeMinDistance = 2.5f;

    [Header("Attacks")]
    [SerializeField] private float closeDistance = 1.5f;
    [SerializeField] private float midDistance = 7f;
    [SerializeField] private float longDistance = 15f;
    
    private SeedLauncher seedLauncher;
    private CameraShake camShake;
    private ChompWave chompWave;

    private AttackStates attackState;
    private enum AttackStates
    {
        None,
        Bite,
        Seeds,
        Wave,
        Charge
    }

    private bool groundAhead;
    private bool wallAhead;

    private bool isRoaming;
    private int roamDirection = 1;

    private bool isCharging;
    private bool isStunned;
    private int chargeDirection;

    private float previousScaleX;
    
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        camShake = Camera.main?.GetComponent<CameraShake>();
        seedLauncher = GetComponentInChildren<SeedLauncher>();

        previousScaleX = transform.localScale.x;
        StartCoroutine(RoamRoutine());
    }

    private void FixedUpdate()
    {
        if (!player) return;

        UpdateChecks();

        if (isStunned)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isCharging)
        {
            ChargeMove();
            return;
        }

        Move();
    }

    private void UpdateChecks()
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

    private void Move()
    {
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        // PLAYER IN VIEW → CHASE
        if (distanceToPlayer <= viewRange)
        {
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
            {
                moveSpeed *= distanceToPlayer / slowDistance;
            }

            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            anim.SetBool("isWalking", true);
            return;
        }

        // PLAYER OUT OF VIEW → ROAM
        isRoaming = true;
        RoamMove();
    }

    private void RoamMove()
    {
        if (wallAhead || !groundAhead)
            roamDirection *= -1;

        rb.linearVelocity = new Vector2(roamDirection * speed, rb.linearVelocity.y);
        FlipWithOffset(roamDirection);
        anim.SetBool("isWalking", true);
    }

    private void ChargeMove()
    {
        rb.linearVelocity = new Vector2(chargeDirection * chargeSpeed, rb.linearVelocity.y);

        if (wallAhead)
        {
            StartCoroutine(StunRoutine());
        }
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true;

        chargeDirection = (int)Mathf.Sign(player.position.x - transform.position.x);
        FlipWithOffset(chargeDirection);

        anim.SetTrigger("charge");

        float timer = 0f;
        while (timer < chargeDuration)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isCharging = false;
    }

    private IEnumerator StunRoutine()
    {
        isCharging = false;
        isStunned = true;

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
    }

    private void FlipWithOffset(float direction)
    {
        float newScaleX = -Mathf.Abs(transform.localScale.x) * direction;

        if (previousScaleX > 0 && newScaleX < 0)
            transform.position += new Vector3(1.12f, 0f, 0f);
        else if (previousScaleX < 0 && newScaleX > 0)
            transform.position += new Vector3(-1.12f, 0f, 0f);

        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        previousScaleX = newScaleX;
    }

    private IEnumerator RoamRoutine()
    {
        while (true)
        {
            if (isRoaming)
            {
                roamDirection = Random.value < 0.5f ? -1 : 1;
                yield return new WaitForSeconds(Random.Range(roamTimeMin, roamTimeMax));
            }
            else
            {
                yield return null;
            }
        }
    }

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

    private void CalcAttack()
    {
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        int randResult = Random.Range(0, 10);
        
        if (distanceToPlayer<closeDistance) //Bite
        {
            attackState = AttackStates.Bite;
        }
        else if (distanceToPlayer<midDistance) //Seed + Wave
        {
            attackState = randResult switch
            {
                < 5 => AttackStates.Seeds,
                < 8 => AttackStates.Wave,
                _ => AttackStates.None
            };
        }
        else if (distanceToPlayer<longDistance) //Wave + Seed + Charge
        {
            attackState = randResult switch
            {
                < 3 => AttackStates.Wave,
                < 5 => AttackStates.Seeds,
                < 6 => AttackStates.Charge,
                _ => AttackStates.None
            };
        }
        else //Charge
        {
            attackState = randResult<6 ? AttackStates.Charge : AttackStates.None;
        }
    }
}
