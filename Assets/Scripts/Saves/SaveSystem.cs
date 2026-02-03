using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class SaveSystem : MonoBehaviour
{
    private static string _savePath;
    public static GameData CurrentData = new GameData();

    public static void SaveToFile()
    {
        SafetyCheck();
        
        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(_savePath, json);
        Debug.Log("Game Saved to: " + _savePath);
    }
    
    public static void LoadFromFile()
    {
        SafetyCheck();

        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            CurrentData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            CurrentData = new GameData();
        }
    }

    public static void ReloadToLastSave()
    {
        SafetyCheck();
        
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            CurrentData = JsonUtility.FromJson<GameData>(json);

            // Reload the current scene
            SceneManager.LoadScene(CurrentData.lastScene);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public static void ClearSave()
    {
        string[] saves =  Directory.GetFiles(Application.persistentDataPath, "*.json");
        foreach (string s in saves)
        {
            File.Delete(s);
        }
        Debug.Log("Save file deleted.");
        
        CurrentData = new GameData();
    }

    public static bool SaveExists()
    {
        string[] saves =  Directory.GetFiles(Application.persistentDataPath, "*.json");
        if (saves.Length != 0)
        {
            return true;
        }
        return false;
    }

    public static void LoadToTheLastSave()
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath, "*.json");
        saves =saves.OrderByDescending(File.GetLastWriteTime).ToArray();
        if (saves.Length != 0)
        {
            _savePath =  saves[0];
        }
        else
        {
            _savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        }
    }

    public static void LoadSpecificSave(int index)
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath, "*.json");

        for (int i =0; i < saves.Length; i++)
        {
            if (i == index)
            {
                _savePath = saves[i];
                return;
            }
        }

        _savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    public static void CreateNewGameSave()
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath, "*.json");

        if (saves.Length != 0)
        {
            _savePath = Path.Combine(Application.persistentDataPath, $"gamesave_{saves.Length}.json");
        }
        else
        {
            _savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        }
    }

    public static void SafetyCheck()
    {
        if (_savePath ==null)
        {
            _savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        }
    }
}