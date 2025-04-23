using UnityEngine;

public class DetachHelmet : MonoBehaviour
{
    public GameObject helmet;
    private Rigidbody2D helmetRb;

    void Start()
    {
        helmet = gameObject;
        if (helmet != null)
        {
            helmetRb = helmet.GetComponent<Rigidbody2D>();
            helmetRb.isKinematic = true;
        }
    }
    
    void Update()
    {
        // Detach helmet from player and enable physics
        
    }
}
