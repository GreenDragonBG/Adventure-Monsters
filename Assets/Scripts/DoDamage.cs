using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoDamage : MonoBehaviour
{
    private float lastDamageTime= -Mathf.Infinity;
    private float damageCooldown = 0.001f;
    private static GameObject _player;
    public static PlayerController _playerController;
    [SerializeField]private bool fromAbove;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && ((Time.time - lastDamageTime) >= damageCooldown))
        {
            DealDamage();
            
            DamageBounce(other);
            
            //tracks the last time taken damage for damage cooldown
            lastDamageTime = Time.time;

        }
    }

    private void Start()
    {
        _player= GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
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
    }

    public static void DealDamage()
    {
        if (_playerController.PlayerHealth-30<=0)
        {
            //player dies and resets the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            _playerController.PlayerHealth = 90;
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
