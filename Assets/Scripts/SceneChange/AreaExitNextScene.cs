using UnityEngine;
namespace SceneChange
{
    public class AreaExitNextScene : AreaExit
    {
        [SerializeField] Vector3 nextStartPos;
        private void Start()
        {
            isNextScene = true;
        }
        
        protected override void OnCameraDisabled()
        {
            
            Debug.Log(nextStartPos);
            PlayerSpawnScript.SpawnPos = nextStartPos;
            CameraController.SpawnPos= Vector3.zero;
            base.OnCameraDisabled();
        }
    }
}