using System;
using System.Collections;
using System.Collections.Generic;
using Mush.Enemies;
using UnityEngine;

public class MushSlug : MonoBehaviour
{
    [Header("Respawn Values")]
    private static List<MushSlug> _allEnemies; 
    private static List<Vector3> _allEnemiesPos;
    private static List<Quaternion> _allEnemiesRot; // Slugs need rotation saved too!
    
    [Header("Stats")]
    [SerializeField] private int maxHealth = 60;
    private int _health;
    private Animator _anim;
    private bool _isDeath;
    private const float Speed = 1f;
    private float _direction = -1;
    
    private CapsuleCollider2D _deathCollider;
    private Rigidbody2D _rb;
    
    // Layer mask for ground
    private int _groundLayerMask;

    private void Awake()
    {
        // Initialize lists only if they are null
        if (_allEnemies == null) _allEnemies = new List<MushSlug>();
        if (_allEnemiesPos == null) _allEnemiesPos = new List<Vector3>();
        if (_allEnemiesRot == null) _allEnemiesRot = new List<Quaternion>();

        _allEnemies.Add(this);
        _allEnemiesPos.Add(transform.position);
        _allEnemiesRot.Add(transform.rotation);
        
        _groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _deathCollider = GetComponent<CapsuleCollider2D>();
        
        ResetSlugState();
    }

    // Helper to set default values
    private void ResetSlugState()
    {
        _health = maxHealth;
        _isDeath = false;
        _direction = transform.localScale.x > 0 ? -1 : 1;
        if(_deathCollider) _deathCollider.enabled = false;
        if(_rb) _rb.bodyType = RigidbodyType2D.Kinematic; // Slugs usually kinematic until death
    }

    private void Update()
    {
        if (!_isDeath && _health <= 0)
        {
            StartCoroutine(Death());
        }
    }

    void FixedUpdate()
    {
        if(_isDeath) return; 
        
        Vector2 downRayOrigin = transform.position + transform.TransformDirection(new Vector2(0, -0.2f));
        Vector2 localDown  = transform.TransformDirection(Vector2.down);
        Vector2 localRight = transform.TransformDirection(Vector2.right);
        Vector2 localLeft  = transform.TransformDirection(Vector2.left);

        RaycastHit2D groundCheck = Physics2D.Raycast(downRayOrigin, localDown, 0.3f, _groundLayerMask);

        if (groundCheck.collider != null)
        {
            transform.Translate(Vector3.right * (_direction * Speed * Time.deltaTime), Space.Self);
            return;
        }

        Vector2 sideRayOrigin = downRayOrigin + localDown * 0.5f; 
        float scaleX = transform.localScale.x;
        float scaleY = transform.localScale.y;

        Vector3 offsetDown = localDown * (0.5f * scaleY);     
        Vector3 offsetSide = localRight * (0.4f * scaleX);   

        RaycastHit2D sideCheck = Physics2D.Raycast(sideRayOrigin, _direction == -1 ? localRight: localLeft, 0.2f , _groundLayerMask);

        if (sideCheck.collider != null)
        {
            transform.Rotate(0, 0, _direction == -1 ? 90f : -90f);
            transform.position += offsetDown - offsetSide;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_isDeath) return; 

        if (other.CompareTag("Ground"))
        {
            transform.Rotate(0, 0,  _direction == -1 ? -90f : 90f);
        }
        else if (other.CompareTag("Attack"))
        {
            _health -= other.transform.parent.GetComponent<PlayerController>().attackDamage;
            _anim.SetTrigger("damaged");
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<Animator>().SetTrigger("Damage");
            DoDamage.DealDamage();
        }
    }

    private IEnumerator Death()
    {
        _isDeath = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _deathCollider.enabled = true;
        
        yield return new WaitForSeconds(2f);
        
        // IMPORTANT: Use SetActive(false) instead of Destroy
        gameObject.SetActive(false);
    }
    
    public static void Respawn()
    {
        for (int i = 0; i < _allEnemies.Count; i++)
        { 
            MushSlug slug = _allEnemies[i];
            
            if(!slug._isDeath) continue;
            slug.gameObject.SetActive(true); 
            slug.enabled = true;
            
            // Reset transforms
            slug.transform.position = _allEnemiesPos[i];
            slug.transform.rotation = _allEnemiesRot[i];
            
            // Reset Physics & Logic
            slug.ResetSlugState();
            
            // Reset Animator
            if(slug._anim != null)
            {
                slug._anim.Rebind();
                slug._anim.Update(0f);
            }

            // Ensure colliders are correct (Enable main, disable death)
            foreach (Collider2D c2D in slug.GetComponents<Collider2D>())
            {
                // If it's the capsule death collider, keep it off for now
                c2D.enabled = (c2D != slug._deathCollider);
            }
        }
    }
}