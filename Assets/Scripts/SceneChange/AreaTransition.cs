using UnityEngine;

namespace SceneChange
{
    public abstract class AreaTransition : MonoBehaviour
    {
        [SerializeField] public bool toMoveLeft = false;
        protected CameraController cam;
        protected PlayerController player;
        protected bool isActive = false; // only true after player enters

        protected virtual void Awake()
        {
            if (Camera.main != null)
            {
                cam = Camera.main.GetComponent<CameraController>();
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            }
        }

        protected virtual void Update()
        {
            // Only move the player if this transition is active and camera is disabled
            if (isActive && !cam.enabled)
            {
                player.HorizontalmoveInput = toMoveLeft ? -1 : 1;
                OnCameraDisabled();
            }
        }

        // Child scripts will override this
        protected abstract void OnCameraDisabled();
    }
}