using UnityEngine;

public class DoDamage : MonoBehaviour
{
    private float lastDamageTime= -Mathf.Infinity;
    private float damageCooldown = 0.001f;
    private float movementCooldown = 0.5f;
    private static GameObject _player;
    private static PlayerController _playerController;
    private static Animator _animator;
    [SerializeField]private bool fromAbove;
    
    private void Start()
    {
        _player= GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        _animator = _player.GetComponent<Animator>();
    }

    public void Update()
    {
            if (gameObject.transform.position.y - _player.transform.position.y > 0.5f)
            {
                fromAbove = true;
            }
            else
            {
                fromAbove = false;
            }
            
            if (!float.IsNegativeInfinity(lastDamageTime) && _playerController.isGrounded && Time.time - lastDamageTime>= movementCooldown)
            {
                _playerController.canMove = true;
            }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && ((Time.time - lastDamageTime) >= damageCooldown))
        {
            _animator.SetTrigger("Damage");
            _playerController.canMove = false;
            DealDamage();
            DamageBounce(other);
            
            //tracks the last time taken damage for damage cooldown
            lastDamageTime = Time.time;

        }
    }


    public static void DealDamage()
    {
        if (_playerController.PlayerHealth-30<=0)
        {
            _animator.SetBool("isDeath", true);
            _player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            // player health is lowered
            _playerController.PlayerHealth -= 30;
        }
    }

    private void DamageBounce(Collider2D playerCollider)
    {
        //Finding player Rigidbody
        Rigidbody2D rb = playerCollider.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
            if (fromAbove)
            {
                //if Enemy attacks from above
                EnemyScript enemyScript = gameObject.transform.GetComponent<EnemyScript>();
                rb.AddForce(
                    enemyScript.hitFromBihind ? 
                        //if Enemy attacks with its back
                        new Vector2((5 * -enemyScript.direction), -2)
                        //if Enemy attacks with its front
                        : new Vector2((5 * enemyScript.direction), -2)
                    , ForceMode2D.Impulse);
            }
            else
            {
                //if Enemy attacks  
                EnemyScript enemyScript = gameObject.transform.GetComponent<EnemyScript>();
                rb.AddForce(
                    enemyScript.hitFromBihind ? 
                        //if Enemy attacks with its back
                        new Vector2((7 * -enemyScript.direction), 10)
                        //if Enemy attacks with its front
                        : new Vector2((7 * enemyScript.direction), 10)
                    , ForceMode2D.Impulse);
            }
    }
}
