using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    private float oldPosition;

    private void Start()
    {
        // Initialize the position tracker at start
        oldPosition = transform.position.x;
    }

    private void Update()
    {
        float current = transform.position.x;
        if (!Mathf.Approximately(current, oldPosition))
        {
            float delta = oldPosition - current;
            onCameraTranslate?.Invoke(delta);
            oldPosition = current;
        }
    }
}