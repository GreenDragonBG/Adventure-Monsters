using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxLayer : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string layerID;

    [Header("Parallax")]
    [SerializeField] public float parallaxFactor;

    private void Start()
    {
        if (!Application.isPlaying) return;
        //LoadState();
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;
        transform.localPosition = newPos;
    }

    public void SaveState()
    {
        string key = "ParallaxLayer_" + layerID;
        int index = SaveSystem.CurrentData.parallaxKeys.IndexOf(key);

        if (index != -1)
        {
            // Update existing entry
            SaveSystem.CurrentData.parallaxValues[index] = transform.position;
        }
        else
        {
            // Add new entry
            SaveSystem.CurrentData.parallaxKeys.Add(key);
            SaveSystem.CurrentData.parallaxValues.Add(transform.position);
        }
    }

    private void LoadState()
    {
        bool isLoadingSave = SaveSystem.CurrentData != null && 
                             !SaveSystem.CurrentData.isNewGame && 
                             SceneManager.GetActiveScene().name == SaveSystem.CurrentData.lastScene;
        if (isLoadingSave)
        {
            string key = "ParallaxLayer_" + layerID;
            int index = SaveSystem.CurrentData.parallaxKeys.IndexOf(key);

            if (index != -1)
            {
                transform.position = SaveSystem.CurrentData.parallaxValues[index];
                Debug.Log($"Loaded parallax position for {layerID}");
            }
        }
    }
}