using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenue : MonoBehaviour
{
    [SerializeField] public GameObject loadButtonPrefab;
    [SerializeField] public Transform contentParent;

    private void Start()
    {
        Load();
    }

    private void Load()
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath, "*.json");
        string[] imgs = Directory.GetFiles(Application.persistentDataPath+"/Screenshots", "*.png");
        saves =saves.OrderByDescending(File.GetLastWriteTime).ToArray();
        imgs = imgs.OrderByDescending(File.GetLastWriteTime).ToArray();

        for (int i =0; i < saves.Length; i++)
        {
            GameObject newButton = Instantiate(loadButtonPrefab, contentParent);
            newButton.transform.localPosition = new Vector2(newButton.transform.localPosition.x, newButton.transform.localPosition.y+(i*-50));
            newButton.GetComponentInChildren<TextMeshProUGUI>().text =Path.GetFileNameWithoutExtension(saves[i]);
            
            byte[] fileData = File.ReadAllBytes(imgs[i]);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                newButton.GetComponentsInChildren<Image>()[^1].sprite =  Sprite.Create(tex, new Rect(702, 320, tex.width/4f, tex.height/4f),new Vector2(0.5f, 0.5f));
            }
            int index = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => LoadGame(index));
        }
    }
    
    public void LoadGame(int index)
    {
        SaveSystem.LoadSpecificSave(index);
        SaveSystem.LoadFromFile();
        SceneManager.LoadScene(SaveSystem.CurrentData.lastScene);
    }
}
