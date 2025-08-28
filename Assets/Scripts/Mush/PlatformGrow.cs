using UnityEngine;

public class PlatformGrow : MonoBehaviour
{
    private float currentTime;
    private float timeDelay = 0.01f;
    private float scaleGrowt = 0.001f;
    private Collider2D platformCollider;
    public static bool IsActive = false;
    void Start()
    {
        transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        currentTime = Time.time;
        platformCollider = GetComponent<Collider2D>();
        platformCollider.enabled = false;
    }
    
    void Update()
    {
        if (IsActive && Time.time - currentTime > timeDelay && transform.localScale.x <= 0.2f)
        {
            currentTime = Time.time;
            transform.localScale = new Vector3(transform.localScale.x + scaleGrowt, transform.localScale.y + scaleGrowt, 1f);
        }
        
        if(transform.localScale.x >= 0.2f){
            platformCollider.enabled = true;
        }
    }
}
