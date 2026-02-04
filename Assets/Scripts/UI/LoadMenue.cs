using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenue : MonoBehaviour
{
    [SerializeField] public StartMenue startMenu;
    [SerializeField] public GameObject loadButtonPrefab;
    [SerializeField] public Transform contentParent;

    private void Start()
    {
        Load();
    }

    private void Load()
    {
        // Get all save folders
        string[] folders = SaveSystem.GetSaveFolders();

        for (int i = 0; i < folders.Length; i++)
        {
            string folderPath = folders[i];
            string saveName = Path.GetFileName(folderPath);
            string jsonPath = Path.Combine(folderPath, saveName + ".json");
            string pngPath = Path.Combine(folderPath, saveName + ".png");

            // Create UI Button
            GameObject newButton = Instantiate(loadButtonPrefab, contentParent);
            newButton.transform.localPosition = new Vector2(newButton.transform.localPosition.x, -25+((i) * -50));
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = saveName;

            // Load Screenshot from inside the folder
            if (File.Exists(pngPath))
            {
                byte[] fileData = File.ReadAllBytes(pngPath);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(fileData))
                {
                    // Note: Ensure your Rect values match your UI needs
                    newButton.GetComponentsInChildren<Image>()[^1].sprite = Sprite.Create(
                        tex, 
                        new Rect(702, 320, tex.width / 3.5f, tex.height / 3.5f),
                        new Vector2(0.5f, 0.5f)
                    );
                }
            }

            int index = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => LoadGame(index));
        }
    }
    
    private void LoadGame(int index)
    {
        SaveSystem.LoadSpecificSave(index);
        SaveSystem.LoadFromFile();
        SceneManager.LoadScene(SaveSystem.CurrentData.lastScene);
    }

    public void ExitButton()
    {
        gameObject.SetActive(false);
    }
}
