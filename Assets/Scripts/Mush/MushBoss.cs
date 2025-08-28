using Mush;
using Unity.VisualScripting;
using UnityEngine;

public class MushBoss : MonoBehaviour
{
    [SerializeField] public float attackInterval= 10;

    private float attackSize;
    private float normalSize;
    [SerializeField] private float sizeChangerValue;
    private float timeScinceAttack;
    private bool isAttacking;
    private Rigidbody2D rb;
    private MushOrb[] orbs;
    private bool chargeLeft = true;
    private MushSpores spores;

    void Start()
    {
        //sets the orbs and spores game objects and turns the orbs off
        spores = transform.parent.GetComponentInChildren<MushSpores>();
        orbs = transform.parent.GetComponentsInChildren<MushOrb>();
        foreach (MushOrb orb in orbs)
        {
            orb.gameObject.SetActive(false);
        }
        //sets sizes on attack
        attackSize = 1.2f;
        normalSize = 1f;
        rb = GetComponent<Rigidbody2D>();
    }
    //Constantly keeps track of the size and attacks on every time intreval
    private void Update()
    {
        ChangeSize();
        
        if (((Time.time - timeScinceAttack) >= attackInterval) && !isAttacking)
        {
            Attack();
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
    
    //sends out the orbs and calculates if it has to send the spores too
    private void Attack()
    {
        timeScinceAttack = Time.time;
        isAttacking = true;
        
        MushOrb.LaunchAll(orbs);
        spores.CalculateChance();
    }
    
}