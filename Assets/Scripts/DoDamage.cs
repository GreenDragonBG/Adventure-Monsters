using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoDamage : MonoBehaviour
{
    private float _lastDamageTime= -Mathf.Infinity;
    private float _damageCooldown = 0.05f;
    private float _lastMoveDirection;
    private GroundSensor groundSensor;
    private  GameObject helmet;
    private Rigidbody2D helmetRb;
    [SerializeField]private GameObject player;
    [SerializeField]private bool fromAbove;
    [SerializeField]private bool isEnemy;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && ((Time.time - _lastDamageTime) >= _damageCooldown))
        {
            
            DamageBounce(other);
            
            //tracks the last time taken damge for damage cooldown
            _lastDamageTime = Time.time;

            DealtDamage();
        }
    }

    private void Start()
    {
        FindPlayerObjects();
        
        if (helmet != null)
        {
            //finds Rigitbody of helmet
            helmetRb = helmet.GetComponent<Rigidbody2D>();
            helmetRb.isKinematic = true;
        }
    }

    public void Update()
    {
        //Gets last faced direction on ground for bounce direction
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0 && groundSensor.m_root.isGrounded)
        {
            _lastMoveDirection = -moveInput;
            
        }

        if (isEnemy)
        {
            if (gameObject.transform.position.y - player.transform.position.y > 0.5f)
            {
                fromAbove = true;
            }
            else
            {
                fromAbove = false;
            }
        }
    }
    
    public  void DealtDamage()
    {
        if (helmet != null)
        {
            //helmet block damage and falls of
            helmet.transform.parent = null;
            helmetRb.isKinematic = false;
            helmetRb.AddForce(new Vector2(-2*_lastMoveDirection,4), ForceMode2D.Impulse);
            helmet = null;
        }
        else if (Swordman.playerHealth-30<=0)
        {
            //player dies and resets the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Swordman.playerHealth = 90;
        }
        else
        {
            // player health is lowered
            Swordman.playerHealth -= 30;
        }
    }

    public void DamageBounce(Collider2D playerCollider)
    {
        //Finding player Rigidbody
        Rigidbody2D rb = playerCollider.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        
        if (fromAbove)
        {
            //if Enemy attacks from above
            if (isEnemy)
            {
                EnemyScript enemyScript = gameObject.transform.GetComponent<EnemyScript>();
                if (enemyScript.hitFromBihind)
                { 
                    //if Enemy attacks with its back
                    rb.AddForce(new Vector2((5*-enemyScript.direction), -2), ForceMode2D.Impulse);
                }
                else
                {
                    //if Enemy attacks with its front
                    rb.AddForce(new Vector2((5*enemyScript.direction), -2), ForceMode2D.Impulse);
                }

            }
            else
            {
                //Force for damage taken form top spikes
                rb.AddForce(new Vector2((5*_lastMoveDirection), -2), ForceMode2D.Impulse);
            }

        }
        else
        {
            //if Enemy attacks   
            if (isEnemy)
            {
                EnemyScript enemyScript = gameObject.transform.GetComponent<EnemyScript>();
                if (enemyScript.hitFromBihind)
                { 
                    //if Enemy attacks with its back
                    rb.AddForce(new Vector2((7 * -enemyScript.direction), 10), ForceMode2D.Impulse);
                }
                else
                {
                    //if Enemy attacks with its front
                    rb.AddForce(new Vector2((7 * enemyScript.direction), 10), ForceMode2D.Impulse);
                }
            }
            else
            {
                //Force for normal taken damage
                rb.AddForce(new Vector2((7*_lastMoveDirection), 10), ForceMode2D.Impulse);
            }
            
        }
    }

    public void FindPlayerObjects()
    {
        for (int i =0; i< player.transform.childCount; i++) {
            if (player.transform.GetChild(i).name =="GroundSensor")
            {
                //Found GrgroundSensor
                groundSensor = player.transform.GetChild(i).GetComponent<GroundSensor>();
            }
            if (player.transform.GetChild(i).name =="model")
            {
                //Found player Model
                for (int j =0; j< player.transform.GetChild(i).childCount; j++) {
                    if (player.transform.GetChild(i).GetChild(j).name =="body")
                    {
                        //Found player Body
                        for (int z =0; z< player.transform.GetChild(i).GetChild(j).childCount; z++) {
                            if (player.transform.GetChild(i).GetChild(j).GetChild(z).name =="Head")
                            { 
                                //Found player Head
                                helmet = player.transform.GetChild(i).GetChild(j).GetChild(z).GetChild(0).gameObject;
                                
                            }
                    
                        }
                        
                    }
                    
                }

            }
        }
    }
}
