using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class SaveSystem : MonoBehaviour
{
    public static string SavePath;
    public static GameData CurrentData = new GameData();

    public static void SaveToFile()
    {
        SafetyCheck();
        
        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game Saved to: " + SavePath);
    }
    
    public static void LoadFromFile()
    {
        SafetyCheck();

        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
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
        
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
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
            SavePath =  saves[0];
        }
        else
        {
            SavePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        }
    }

    public static void LoadSpecificSave(int index)
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath, "*.json");

        for (int i =0; i < saves.Length; i++)
        {
            if (i == index)
            {
                SavePath = saves[i];
                return;
            }
        }

        SavePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    public static void CreateNewGameSave()
    {
        string[] saves = Directory.GetFiles(Application.persistentDataPath, "*.json");

        if (saves.Length != 0)
        {
            SavePath = Path.Combine(Application.persistentDataPath, $"gamesave_{saves.Length}.json");
        }
        else
        {
            SavePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        }
    }

    public static void SafetyCheck()
    {
        if (SavePath ==null)
        {
            SavePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        }
    }
}