using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class EscapeMenu : MonoBehaviour
    {
        [SerializeField] GameObject optionsMenu;
        [SerializeField] AffirmationMenu affirmationMenu;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !affirmationMenu.gameObject.activeSelf)
            {
                if (optionsMenu.activeSelf)
                {
                    optionsMenu.SetActive(false);
                }
                else
                {
                    Continue();
                }
            }
        }

        public void Continue()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
        public void MainMenu()
        {
            affirmationMenu.result = -1;
            affirmationMenu.gameObject.SetActive(true);
            StartCoroutine(WaitForMainMenuResult());
        }
        public void Options()
        {
            optionsMenu.SetActive(true);
        }
        public void ExitGame()
        {
            affirmationMenu.result = -1;
            affirmationMenu.gameObject.SetActive(true);
            StartCoroutine(WaitForExitGameResult());
        }

        private IEnumerator WaitForMainMenuResult()
        {
            while (affirmationMenu.result==-1)
            {
                yield return null;
            }

            if (affirmationMenu.result == 1)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
            }
            
            affirmationMenu.gameObject.SetActive(false);
        }

        private IEnumerator WaitForExitGameResult()
        {
            while (affirmationMenu.result==-1)
            {
                yield return null;
            }

            if (affirmationMenu.result == 1)
            {
                Time.timeScale = 1f;
                Application.Quit();
            }
            
            affirmationMenu.gameObject.SetActive(false);
        }
    }
}
