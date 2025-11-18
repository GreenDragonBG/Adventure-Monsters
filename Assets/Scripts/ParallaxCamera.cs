using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate OnCameraTranslate;

    private float oldPosition;

    private void Awake()
    {
        // capture the position as early as possible
        oldPosition = transform.position.x;
    }

    private void Start()
    {
        // If some other Start() moved the camera before this Start(), detect it now:
        float current = transform.position.x;
        if (!Mathf.Approximately(current, oldPosition))
        {
            float delta = oldPosition - current;
            OnCameraTranslate?.Invoke(delta);
            // update oldPosition so following frames compute deltas correctly
            oldPosition = current;
        }
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