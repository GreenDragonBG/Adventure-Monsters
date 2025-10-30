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
            cam.enabled = false;
            isActive = true; // entrance starts active
        }
        private void Start()
        {
            bool isPlayerLeft = player.gameObject.transform.position.x < transform.position.x;
            if (isPlayerLeft == toMoveLeft)
            {
                disableEntrance();
            }
        }

        protected override void OnCameraDisabled() { }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                disableEntrance();
            }
        }

        private void disableEntrance()
        {
            Debug.Log("Disabling Area");
            cam.enabled = true;

            // Enable corresponding exit if exists
            if (exit != null) exit.enabled = true;

            // Disable this entrance to prevent conflicts
            isActive = false;
            gameObject.SetActive(false);
        }
    }
}