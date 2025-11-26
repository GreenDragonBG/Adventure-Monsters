using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneChange
{
    public abstract class AreaExit : AreaTransition
    {
        protected bool IsNextScene = true;
        [SerializeField] public float timeTillNextScene = 0f;
        private float timeExitedArea = -Mathf.Infinity;
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Cam.enabled = false;
                timeExitedArea = Time.time;
                IsActive = true; // now this exit is active
            }
        }

        protected override void OnCameraDisabled()
        {
            if (Time.time - timeExitedArea > timeTillNextScene)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + (IsNextScene ? 1 : -1));
            }
        }
        
    }
}