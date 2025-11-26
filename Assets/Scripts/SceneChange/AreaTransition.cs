using UnityEngine;

namespace SceneChange
{
    public abstract class AreaTransition : MonoBehaviour
    {
        [SerializeField] public bool toMoveLeft = false;
        protected CameraController Cam;
        protected PlayerController Player;
        protected bool IsActive = false; // only true after player enters

        protected virtual void Awake()
        {
            if (Camera.main != null)
            {
                Cam = Camera.main.GetComponent<CameraController>();
                Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            }
        }

        protected virtual void Update()
        {
            // Only move the player if this transition is active and camera is disabled
            if (IsActive && !Cam.enabled)
            {
                Player.horizontalmoveInput = toMoveLeft ? -1 : 1;
                OnCameraDisabled();
            }
        }

        // Child scripts will override this
        protected abstract void OnCameraDisabled();
    }
}