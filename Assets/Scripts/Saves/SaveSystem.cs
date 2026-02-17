using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public static class SaveSystem
{
    public static string SavePath;
    public static GameData CurrentData = new GameData();

    // Helper to get the folder containing the current JSON
    public static string GetCurrentSaveDirectory()
    {
        SafetyCheck();
        return Path.GetDirectoryName(SavePath);
    }

    public static void SaveToFile()
    {
        SafetyCheck();
        
        // Ensure the specific folder for this save exists
        string folder = Path.GetDirectoryName(SavePath);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(SavePath, json);
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

    // New logic: Look for folders instead of loose files
    public static string[] GetSaveFolders()
    {
        if (!Directory.Exists(Application.persistentDataPath)) return Array.Empty<string>();
        
        // We look for directories that contain a .json file inside them
        return Directory.GetDirectories(Application.persistentDataPath)
            .OrderByDescending(Directory.GetLastWriteTime)
            .ToArray();
    }

    public static void CreateNewGameSave()
    {
        int saveCount = GetSaveFolders().Length;
        string saveName = $"gamesave_{saveCount}";
        string folderPath = Path.Combine(Application.persistentDataPath, saveName);
        SavePath = Path.Combine(folderPath, saveName + ".json");
    }

    public static void LoadSpecificSave(int index)
    {
        string[] folders = GetSaveFolders();
        if (index >= 0 && index < folders.Length)
        {
            string folderName = Path.GetFileName(folders[index]);
            SavePath = Path.Combine(folders[index], folderName + ".json");
        }
        else
        {
            CreateNewGameSave();
        }
    }

    public static void LoadToTheLastSave()
    {
        string[] folders = GetSaveFolders();
        if (folders.Length > 0)
        {
            string folderName = Path.GetFileName(folders[0]); // Most recent folder
            SavePath = Path.Combine(folders[0], folderName + ".json");
        }
        else
        {
            CreateNewGameSave();
        }
    }
    
    public static void ClearSave()
    {
        // Get all directories in the persistent data path
        string[] saveFolders = Directory.GetDirectories(Application.persistentDataPath);

        foreach (string folder in saveFolders)
        {
            try 
            {
                Directory.Delete(folder, true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete folder {folder}: {e.Message}");
            }
        }
    
        // Reset data to defaults
        CurrentData = new GameData();
        SavePath = null;
    }
    
    public static void ReloadToLastSave()
    {
        // 1. Refresh SavePath to point to the most recent folder/file
        LoadToTheLastSave();
    
        // 2. Check if that file actually exists
        if (File.Exists(SavePath))
        {
            // 3. Read the JSON
            string json = File.ReadAllText(SavePath);
            CurrentData = JsonUtility.FromJson<GameData>(json);

            // 4. Move the player back to the last saved scene
            if (!string.IsNullOrEmpty(CurrentData.lastScene))
            {
                SceneManager.LoadScene(CurrentData.lastScene);
            }
            else
            {
                // Fallback if scene name is missing
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            // If no saves exist at all, go to the first level
            SceneManager.LoadScene(1);
        }
    }

    public static bool SaveExists() => GetSaveFolders().Length > 0;

    public static void SafetyCheck()
    {
        if (string.IsNullOrEmpty(SavePath))
        {
            string folderPath = Path.Combine(Application.persistentDataPath, "gamesave");
            SavePath = Path.Combine(folderPath, "gamesave.json");
        }
    }
}