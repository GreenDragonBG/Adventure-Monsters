using System;
using UnityEngine;

namespace SceneChange
{
    public class AreaEntrance : AreaTransition
    {
        [SerializeField]public AreaExit exit;
        protected override void Awake()
        {
            base.Awake();
            Cam.enabled = false;
            IsActive = true; // entrance starts active
        }
        private void Start()
        {
            bool isPlayerLeft = Player.gameObject.transform.position.x < transform.position.x;
            if (isPlayerLeft == toMoveLeft)
            {
                DisableEntrance();
            }
        }

        protected override void OnCameraDisabled() { }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                DisableEntrance();
            }
        }

        private void DisableEntrance()
        {
            Debug.Log("Disabling Area");
            Cam.enabled = true;

            // Enable corresponding exit if exists
            if (exit != null) exit.enabled = true;

            // Disable this entrance to prevent conflicts
            IsActive = false;
            gameObject.SetActive(false);
        }
    }
}