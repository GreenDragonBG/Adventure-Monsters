using UnityEngine;
 
[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate OnCameraTranslate;
 
    private float oldPosition;
 
    void Start()
    {
        oldPosition = transform.position.x;
    }
 
    void Update()
    {
        if (transform.position.x != oldPosition)
        {
            if (OnCameraTranslate != null)
            {
                float delta = oldPosition - transform.position.x;
                OnCameraTranslate(delta);
            }
 
            oldPosition = transform.position.x;
        }
    }
}