using Saves;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

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
        
        videoResolution.onValueChanged.AddListener(delegate { OptionsSave.Data.VideoResolution = videoResolution.value; OptionsSave.SaveOptions(); });
        vSync.onValueChanged.AddListener(delegate { OptionsSave.Data.VSync = vSync.isOn; OptionsSave.SaveOptions(); });
        fullscreen.onValueChanged.AddListener(delegate { OptionsSave.Data.Fullscreen = vSync.isOn; OptionsSave.SaveOptions(); });
    }
    
    public void ExitButton()
    {
        gameObject.SetActive(false);
    }
}
