using UnityEngine;
namespace SceneChange
{
    public class AreaExitNextScene : AreaExit
    {
        [SerializeField] Vector3 nextStartPos;

        private void Start()
        {
            IsNextScene = true;
        }

        protected override void OnCameraDisabled()
        {
            PlayerSpawnScript.SpawnPos = nextStartPos;
            CameraController.SpawnPos= Vector3.zero;
            base.OnCameraDisabled();
        }
    }
}