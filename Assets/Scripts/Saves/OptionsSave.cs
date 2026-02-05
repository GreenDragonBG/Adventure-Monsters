using System.IO;
using UnityEngine;

namespace Saves
{
    public static class OptionsSave
    {
        public class OptionsData
        {
            //Volume
            public float MasterVolume = 1;
            public float MusicVolume = 1;
            public float SfxVolume = 1;
            //Accessibility
            public bool CameraShake = true;
            //Video
            public int VideoResolution = 2;
            public bool VSync = true;
            public bool Fullscreen = true;
        }
        
        public static string SavePath => Path.Combine(Application.persistentDataPath, "options.json");
        public static OptionsData Data = new OptionsData();

        public static void SaveOptions()
        {
            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(SavePath, json);
        }

        public static void LoadOptions()
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                Data = JsonUtility.FromJson<OptionsData>(json);
            }
            else
            {
                Data = new OptionsData();
            }
        }
    }
}