using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneChange
{
    public abstract class AreaExit : AreaTransition
    {
        protected bool isNextScene = true;
        [SerializeField] public float timeTillNextScene = 0f;
        private float timeExitedArea = -Mathf.Infinity;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                cam.enabled = false;
                timeExitedArea = Time.time;
                isActive = true; // now this exit is active
            }
        }

        protected override void OnCameraDisabled()
        {
            if (Time.time - timeExitedArea > timeTillNextScene)
            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + (isNextScene ? 1 : -1));
            }
        }
    }
}