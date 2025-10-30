using System;
using UnityEngine;

namespace SceneChange
{
    public class AreaExitPreviousScene: AreaExit
    {
        [SerializeField] Vector3 returnStartPos;
        [SerializeField] Vector3 camPos;

        private void Start()
        {
            isNextScene = false;
        }

        protected override void OnCameraDisabled()
        {
            Debug.Log(returnStartPos);
            PlayerSpawnScript.SpawnPos = returnStartPos;
            CameraController.SpawnPos = camPos;
            base.OnCameraDisabled();
        }
    }
}
