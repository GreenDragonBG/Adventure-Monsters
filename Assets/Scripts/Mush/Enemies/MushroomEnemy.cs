using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mush.Enemies
{
    public class MushroomEnemy : MonoBehaviour
    {
        [Header("Respawn Values")]
        private static List<MushroomEnemy> _allEnemies; 
        private static List<Vector3> _allEnemiesPos;
        private static List<Vector2> _allEnemiesScale; 
    
        [Header("References")]
        private Transform _player;
        private Rigidbody2D _rb;
        private Animator _anim;

        [Header("Health")] 
        public int health = 60;
        private int _currentHealth;

        [Header("Movement")]
        [SerializeField] private float speed = 2f;
        [SerializeField] private float stopDistance = 0.6f;
        [SerializeField] private float slowDistance = 1f;
        [SerializeField] private float viewRange = 5f;

        [Header("Roaming")]
        [SerializeField] private float roamTimeMin = 1f;
        [SerializeField] private float roamTimeMax = 3f;
        private bool _isRoaming;
        private int _roamDirection = 1;

        [Header("Checks")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform wallCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float checkDistance = 0.15f;
        private bool _groundAhead;
        private bool _wallAhead;
    
        [Header("Attack")]
        private float _lastTimeAttacked = -Mathf.Infinity;
        [SerializeField]private float rangeToAttack = 1f;
        private bool _isInAttackRange;

        [Header("Damaged")]
        private float _lastTimeDamaged = -Mathf.Infinity;

        // Animation Hash
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

        private void Awake()
        {
            _allEnemies = new List<MushroomEnemy>();
            _allEnemiesPos = new List<Vector3>();
            _allEnemiesScale = new List<Vector2>();
            foreach (MushroomEnemy e in FindObjectsByType<MushroomEnemy>(FindObjectsInactive.Include,FindObjectsSortMode.InstanceID))
            {
                _allEnemies.Add(e);
                _allEnemiesPos.Add(e.transform.position);
                _allEnemiesScale.Add(e.transform.localScale);
            }
        }

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();

            if (_player == null) 
                _player = GameObject.FindGameObjectWithTag("Player")?.transform;

            _currentHealth = health;
        }
        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(RoamRoutine());
        }
        
        void FixedUpdate()
        {
            if (!_player) return;

            UpdateChecks();
            Move();
            TriggerAttack();
        }

        private void UpdateChecks()
        {
            _groundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
            _wallAhead = Physics2D.Raycast(wallCheck.position, transform.right*_roamDirection, checkDistance, groundLayer);
        }

        private void Move()
        {
            float distanceToPlayer = Mathf.Abs(_player.position.x - transform.position.x);

            if (distanceToPlayer <= viewRange)
            {
                _isRoaming = false;

                float direction = Mathf.Sign(_player.position.x - transform.position.x);
            
                Flip(direction);

                if (distanceToPlayer <= stopDistance || _wallAhead || !_groundAhead)
                {
                    _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                    _anim.SetBool(IsWalking, false);
                    return;
                }

                float moveSpeed = speed;
                if (distanceToPlayer < slowDistance)
                    moveSpeed *= distanceToPlayer / slowDistance;

                _rb.linearVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);
                _anim.SetBool(IsWalking, true);
            }
            else
            {
                _isRoaming = true;
            }
        }

        private void RoamMove()
        {
            if (_wallAhead || !_groundAhead)
            {
                _roamDirection *= -1;
            }

            _rb.linearVelocity = new Vector2(_roamDirection * speed, _rb.linearVelocity.y);
            Flip(_roamDirection);
            _anim.SetBool(IsWalking, true);
        }

        private IEnumerator RoamRoutine()
        {
            while (true)
            {
                if (_isRoaming)
                {
                    _roamDirection = Random.value < 0.5f ? -1 : 1;
                    float roamDuration = Random.Range(roamTimeMin, roamTimeMax);
                    float elapsed = 0f;

                    while (elapsed < roamDuration && _isRoaming)
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

        private void Flip(float direction)
        {
            float newScaleX = -Mathf.Abs(transform.localScale.x) * direction;
            transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        }

        private void TriggerAttack()
        {
            float distanceToPlayer = Mathf.Abs(_player.position.x - transform.position.x);
            if (distanceToPlayer <= rangeToAttack && Time.time-_lastTimeAttacked>2f)
            {
                _lastTimeAttacked =  Time.time;
                _anim.SetTrigger("Attack");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isInAttackRange = true;
            } else if (other.CompareTag("Attack") && Time.time-_lastTimeDamaged>0.5f)
            {
                _lastTimeDamaged = Time.time;
                _currentHealth -= other.transform.parent.GetComponent<PlayerController>().attackDamage;
                _anim.SetTrigger("Hurt");
                if (_currentHealth<=0)
                {
                    _anim.SetTrigger("Death");
                }
            }else if (other.CompareTag("EnemyAreaEdge"))
            {
                _roamDirection *= -1;
                Flip(_roamDirection);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isInAttackRange = false;
            }
        }

        private void DamagePlayer()
        {
            if(!_isInAttackRange) return;
        
            _player.gameObject.GetComponent<Animator>()?.SetTrigger("Damage");
            DoDamage.DealDamage();
        }

        private void OnDeath()
        {
            _anim.SetBool("isDead", true);
            enabled = false;
            _rb.linearVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            foreach (Collider2D c2D in GetComponents<Collider2D>())
            {
                c2D.enabled = false;
            }
        }

        private void AfterDeath()
        {
            StartCoroutine(AfterDeathCoroutine());
        }

        private IEnumerator AfterDeathCoroutine()
        {
            Vector3 startScale = transform.localScale;
            float direction = Mathf.Sign(startScale.x);
            float currentAbsScale = Mathf.Abs(startScale.x);

            while (currentAbsScale > 0.1f)
            {
                currentAbsScale -= 0.1f;
            
                transform.localScale = new Vector3(
                    currentAbsScale * direction, 
                    currentAbsScale, 
                    startScale.z
                );
        
                yield return new WaitForSeconds(0.05f);
            }
    
            gameObject.SetActive(false);
        }

        public static void Respawn()
        {
            for (int i =0;i < _allEnemies.Count; i++)
            { 
                if(_allEnemies[i]._anim.isActiveAndEnabled && !_allEnemies[i]._anim.GetBool("isDead")) continue;
                _allEnemies[i].gameObject.SetActive(true); 
                _allEnemies[i]._anim.Rebind();
                _allEnemies[i].enabled= true;
                _allEnemies[i].transform.localScale =  _allEnemiesScale[i];
                _allEnemies[i].transform.position = _allEnemiesPos[i];
                foreach (Collider2D c2D in  _allEnemies[i].GetComponents<Collider2D>())
                {
                    c2D.enabled = true;
                }
                _allEnemies[i]._rb.bodyType = RigidbodyType2D.Dynamic;
                _allEnemies[i]._currentHealth = _allEnemies[i].health;
                _allEnemies[i]._isRoaming = true;
            }
            
        }
    }
}