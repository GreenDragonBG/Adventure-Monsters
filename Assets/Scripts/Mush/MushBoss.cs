using Mush;
using Unity.VisualScripting;
using UnityEngine;

public class MushBoss : MonoBehaviour
{
    //Orb vars
    [Header("Orbs")]
    [SerializeField] public float attackInterval= 10;
    private float timeSinceAttack;
    private bool isAttacking;
    private MushOrb[] orbs;
    
    //Size changing on orb attack vars
    [Header("Orbs animation size change")]
    [SerializeField] private float sizeChangerValue;
    private float attackSize;
    private float normalSize;
    
    //Spore vars
    [Header("Spores")]
    [SerializeField] public float sporeInterval = 7f;
    private MushSpores spores;
    private float lastSporeCalculated;

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
    }
    //Constantly keeps track of the size and attacks on every time intreval
    private void Update()
    {
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
    
    //sends out the orbs and calculates if it has to send the spores too
    private void Attack()
    {
        timeSinceAttack = Time.time;
        isAttacking = true;
        
        LaunchAll(orbs);
    }

    private void SporeCalculate()
    {   
        spores.CalculateChance();
        lastSporeCalculated = Time.time;
    }
    
    private void LaunchAll(MushOrb[] orbs)
    {
        //for each orb that is called it gets activated and launched
        foreach (MushOrb orb in orbs)
        {
            orb.gameObject.SetActive(true);
            orb.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(orb.launcForceX, orb.launcForceY);
        }
    }

}