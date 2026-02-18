using Saves;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionsMenu : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider musicVolume;
        [SerializeField] private Slider sfxVolume;
    
        [Header("Accessibility")]
        [SerializeField] private Toggle cameraShake;
    
        [Header("Video")]
        [SerializeField] private TMP_Dropdown videoResolution;
        [SerializeField] private Toggle vSync;
        [SerializeField] private Toggle fullscreen;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            OptionsSave.LoadOptions();
            masterVolume.value = OptionsSave.Data.MasterVolume;
            musicVolume.value = OptionsSave.Data.MusicVolume;
            sfxVolume.value = OptionsSave.Data.SfxVolume;
        
            cameraShake.isOn = OptionsSave.Data.CameraShake;
        
            videoResolution.value = OptionsSave.Data.VideoResolution;
            vSync.isOn = OptionsSave.Data.VSync;
            fullscreen.isOn = OptionsSave.Data.Fullscreen;
        
            masterVolume.onValueChanged.AddListener(delegate { OptionsSave.Data.MasterVolume = masterVolume.value; OptionsSave.SaveOptions(); });
            musicVolume.onValueChanged.AddListener(delegate { OptionsSave.Data.MusicVolume = musicVolume.value; OptionsSave.SaveOptions(); });
            sfxVolume.onValueChanged.AddListener(delegate { OptionsSave.Data.SfxVolume = sfxVolume.value; OptionsSave.SaveOptions();});
        
            cameraShake.onValueChanged.AddListener(delegate { OptionsSave.Data.CameraShake = cameraShake.isOn; OptionsSave.SaveOptions(); });
        
            videoResolution.onValueChanged.AddListener(delegate { OptionsSave.Data.VideoResolution = videoResolution.value; OptionsSave.SaveOptions(); SetScreenResolution();});
            vSync.onValueChanged.AddListener(delegate { OptionsSave.Data.VSync = vSync.isOn; OptionsSave.SaveOptions(); SetVSync();});
            fullscreen.onValueChanged.AddListener(delegate { OptionsSave.Data.Fullscreen = fullscreen.isOn; OptionsSave.SaveOptions(); SetScreenResolution(); });
        }

        private void SetScreenResolution()
        {
            switch (videoResolution.value)
            {
                case 0:
                    Screen.SetResolution(3840, 2160, fullscreen.isOn);
                    break;
                case 1: 
                    Screen.SetResolution(2560, 1440, fullscreen.isOn);
                    break;
                case 2:
                    Screen.SetResolution(1920, 1080, fullscreen.isOn);
                    break;
                case 3:
                    Screen.SetResolution(1600, 900, fullscreen.isOn);
                    break;
                case 4:
                    Screen.SetResolution(1280, 720, fullscreen.isOn);
                    break;
                case 5:
                    Screen.SetResolution(960, 540, fullscreen.isOn);
                    break;
                default:
                    Screen.SetResolution(1920, 1080, fullscreen.isOn);
                    break;
            }
        }
        
        private void SetVSync()
            {
                if (vSync.isOn)
                {
                    QualitySettings.vSyncCount = 1;
                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 60; 
                }

            }

        public void ExitButton()
        {
            gameObject.SetActive(false);
        }
    
    }
}
