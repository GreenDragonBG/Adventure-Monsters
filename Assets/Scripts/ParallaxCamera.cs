using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate OnCameraTranslate;

    private float oldPosition;

    private void Start()
    {
        // Initialize the position tracker at start
        oldPosition = transform.position.x;
    }

    // This allows the CameraController to tell the layers to jump
    public void ForceResetBackground(float delta)
    {
        OnCameraTranslate?.Invoke(delta);
        oldPosition = transform.position.x;
    }

    private void Update()
    {
        float current = transform.position.x;
        if (!Mathf.Approximately(current, oldPosition))
        {
            float delta = oldPosition - current;
            OnCameraTranslate?.Invoke(delta);
            oldPosition = current;
        }
    }
}