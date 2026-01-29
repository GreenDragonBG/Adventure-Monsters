using UnityEngine;
namespace SceneChange
{
    public class AreaExitNextScene : AreaExit
    {
        [SerializeField] Vector3 nextStartPos;
        
        protected override void Awake()
        {
            base.Awake();
            IsNextScene = true;
        }

        protected override void OnCameraDisabled()
        {
            PlayerSpawnScript.SpawnPos = nextStartPos;
            CameraController.SpawnPos = Vector3.zero;
            PlayerController.ShouldTeleportToSave = false;
            base.OnCameraDisabled();
        }
    }
}