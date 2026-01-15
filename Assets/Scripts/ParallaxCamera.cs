using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate OnCameraTranslate;

    private float oldPosition;

    private void Awake()
    {
        // Never restore camera position outside Play Mode
        if (!Application.isPlaying)
        {
            oldPosition = transform.position.x;
            return;
        }

        // Only restore if a camera save exists
        if (PlayerPrefs.HasKey("CameraPosX"))
        {
            float sceneStartX = transform.position.x;

            Vector3 savedPos = new Vector3(
                PlayerPrefs.GetFloat("CameraPosX"),
                PlayerPrefs.GetFloat("CameraPosY"),
                PlayerPrefs.GetFloat("CameraPosZ")
            );

            transform.position = savedPos;

            // Force ONE parallax update
            float delta = sceneStartX - savedPos.x;
            if (!Mathf.Approximately(delta, 0f))
            {
                OnCameraTranslate?.Invoke(delta);
            }
        }

        oldPosition = transform.position.x;
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        float current = transform.position.x;
        if (!Mathf.Approximately(current, oldPosition))
        {
            float delta = oldPosition - current;
            OnCameraTranslate?.Invoke(delta);
            oldPosition = current;
        }
    }
}