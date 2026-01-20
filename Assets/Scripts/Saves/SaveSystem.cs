using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "gamesave.json");
    public static GameData CurrentData = new GameData();

    public static void SaveToFile()
    {
        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game Saved to: " + SavePath);
    }

    public static void LoadFromFile()
    {
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
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            CurrentData = JsonUtility.FromJson<GameData>(json);

            // Reload the current scene
            SceneManager.LoadScene(CurrentData.lastScene);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }


    // --- NEW METHOD ---
    public static void ClearSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted.");
        }

        // Reset the data in RAM so the current session starts fresh
        CurrentData = new GameData();
    }
}