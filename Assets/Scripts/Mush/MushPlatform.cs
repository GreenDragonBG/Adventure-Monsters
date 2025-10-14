using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MushPlatform : MonoBehaviour
{
    private float currentTime;
    private float timeDelay = 0.01f;
    private float scaleGrowt = 0.001f;
    private Collider2D platformCollider;
    public static bool IsActive = false;
    private Light2D[] lights;
    [SerializeField]public Transform playerTransform;
    void Start()
    {
        lights= GetComponentsInChildren<Light2D>();
        transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        currentTime = Time.deltaTime;
        platformCollider = GetComponent<Collider2D>();
        platformCollider.enabled = false;
    }
    
    void Update()
    {
        CheckToGorwPlatform();
        CheckToLowerPlatform();
        
        if(transform.localScale.x >= 0.2f && IsPlayerAboveCollider()){
            platformCollider.enabled = true;
        }
        else
        {
            platformCollider.enabled = false;
        }
    }

    private void CheckToGorwPlatform()
    {
        if (IsActive && Time.time - currentTime > timeDelay && transform.localScale.x <= 0.2f)
        {
            currentTime = Time.time;
            transform.localScale = new Vector3(transform.localScale.x + scaleGrowt, transform.localScale.y + scaleGrowt, 1f);
            foreach (Light2D lighting in lights)
            {
                lighting.enabled = true;
            }
        }
    }
    private void CheckToLowerPlatform()
    {
        if (!IsActive && Time.time - currentTime > timeDelay && transform.localScale.x >= 0.01f)
        {
            currentTime = Time.time;
            transform.localScale = new Vector3(transform.localScale.x - scaleGrowt, transform.localScale.y - scaleGrowt, 1f);
            foreach (Light2D lighting in lights)
            {
                lighting.enabled = false;
            }
        }
    }

    private bool IsPlayerAboveCollider()
    {
        if (playerTransform.position.y > transform.position.y+2)
        {
            return true;
        }
        return false;
    }
}
