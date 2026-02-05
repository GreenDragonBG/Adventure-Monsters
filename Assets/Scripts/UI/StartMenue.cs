using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenue : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuNoSaves;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loadMenu;
    [SerializeField] private GameObject optionsMenu;

    [Header("Cloud Layers")]
    [SerializeField] private CloudSettings layer1;
    [SerializeField] private CloudSettings layer2;
    [SerializeField] private CloudSettings layer3;

    [System.Serializable]
    public class CloudSettings {
        public GameObject parent;
        public float minX;
        public float maxX;
        public float speed;
        [HideInInspector] public Transform[] transforms;
    }

    private void Awake()
    {
        MainMenuSetUp();
    }

    private void Start()
    {
        PlayerController.ShouldTeleportToSave = true;
        // Initialize and start all three layers
        SetupAndStartLayer(layer1);
        SetupAndStartLayer(layer2);
        SetupAndStartLayer(layer3);
    }

    private void SetupAndStartLayer(CloudSettings layer) {
        if (layer.parent != null) {
            layer.transforms = layer.parent.GetComponentsInChildren<Transform>();
            StartCoroutine(MoveCloudLayer(layer));
        }
    }

    private IEnumerator MoveCloudLayer(CloudSettings layer)
    {
        while (true)
        {
            foreach (Transform cloud in layer.transforms)
            {
                if (cloud == layer.parent.transform) continue;

                Vector3 pos = cloud.position;
                pos.x -= layer.speed;
                
                if (pos.x < layer.minX) {
                    pos.x = layer.maxX;
                }

                cloud.position = pos;
            }
            yield return new WaitForSeconds(0.04f);
        }
    }

    public void MainMenuSetUp()
    {
        mainMenuNoSaves.SetActive(false);
        mainMenu.SetActive(false);
        
        if (SaveSystem.SaveExists()) {
            mainMenu.SetActive(true);
        } else {
            mainMenuNoSaves.SetActive(true);
        }
        loadMenu.SetActive(false);
    }

    public void NewGameButton()
    {
        SaveSystem.CreateNewGameSave();
        SceneManager.LoadScene(1);
    }

    public void ContinueButton()
    {
        SaveSystem.LoadToTheLastSave();
        SaveSystem.LoadFromFile();
        SceneManager.LoadScene(SaveSystem.CurrentData.lastScene);
    }
    
    public void LoadButton()
    {
        loadMenu.SetActive(true);
    }

    public void OptionsButton()
    {
        optionsMenu.SetActive(true);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}