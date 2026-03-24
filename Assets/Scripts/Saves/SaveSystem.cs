using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class SaveSystem
{
    public static string SavePath;
    public static GameData CurrentData = new GameData();
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("[jAn2LZoUIf;5UJKCRSH*7Jg!,O:J2Y6");

    public static void SaveToFile()
    {
        SafetyCheck();
        
        // Ensure the specific folder for this save exists
        string folder = Path.GetDirectoryName(SavePath);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string json = JsonUtility.ToJson(CurrentData, true);

        //Encryption off the file and then save
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.GenerateIV(); // Unique IV for every save file
        byte[] iv = aes.IV;

        using FileStream fs = new FileStream(SavePath, FileMode.Create);
        //Write the IV to the start of the file (16 bytes)
        fs.Write(iv, 0, iv.Length);

        // Wrap the stream in a CryptoStream to encrypt the rest
        using CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);

        using StreamWriter sw = new StreamWriter(cs);
        sw.Write(json);
    }

    public static void LoadFromFile()
    {
        SafetyCheck();
        if (!File.Exists(SavePath))
        {
            CurrentData = new GameData();
            return;
        }
        
        //Decryption of the file
        try
        {
            using Aes aes = Aes.Create();
            aes.Key = Key;
            byte[] iv = new byte[16];

            using FileStream fs = new FileStream(SavePath, FileMode.Open);
            //Read the 16-byte IV back from the start of the file
            fs.Read(iv, 0, iv.Length);
            aes.IV = iv;

            //Decrypt the remaining data
            using (CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                string json = sr.ReadToEnd();
                CurrentData = JsonUtility.FromJson<GameData>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to decrypt save: {e.Message}. Data may be corrupted or key is wrong.");
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
            // 3. Read the JSON by decryption
            try
            {
                using Aes aes = Aes.Create();
                aes.Key = Key;
                byte[] iv = new byte[16];

                using FileStream fs = new FileStream(SavePath, FileMode.Open);
                //Read the 16-byte IV back from the start of the file
                fs.Read(iv, 0, iv.Length);
                aes.IV = iv;

                //Decrypt the remaining data
                using (CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    string json = sr.ReadToEnd();
                    CurrentData = JsonUtility.FromJson<GameData>(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save: {e.Message}. Data may be corrupted.");
                CurrentData = new GameData();
            }

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

    private static void SafetyCheck()
    {
        if (string.IsNullOrEmpty(SavePath))
        {
            string folderPath = Path.Combine(Application.persistentDataPath, "gamesave");
            SavePath = Path.Combine(folderPath, "gamesave.json");
        }
    }
}