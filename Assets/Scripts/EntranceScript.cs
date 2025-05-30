using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceScript : MonoBehaviour
{
    [SerializeField] public Vector3 travelPosition;
    [SerializeField] private int sceneToLoad;
    private bool isTriggered;
    private Canvas canvas;

    void Start()
    {
        
        isTriggered = false;
        canvas = gameObject.transform.GetComponentInChildren<Canvas>();
    }

    void Update()
    {
        if (isTriggered)
        {
            canvas.enabled = true;
            
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                PlayerSpawnScript.SpawnPos = travelPosition;
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            canvas.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = false;
        }
    }
}
