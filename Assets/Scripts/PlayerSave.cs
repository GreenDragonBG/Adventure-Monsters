using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSave : MonoBehaviour
{
    private const string PosX = "PlayerPosX";
    private const string PosY = "PlayerPosY";
    private const string PosZ = "PlayerPosZ";
    private const string SceneKey = "SavedScene";

    private const string CamX = "CameraPosX";
    private const string CamY = "CameraPosY";
    private const string CamZ = "CameraPosZ";

    private ParallaxLayer[] parallaxLayers;

    private void Awake()
    {
        // 🔑 Make sure the player has NO parent
        transform.SetParent(null);
    }

    private void Start()
    {
        parallaxLayers = FindObjectsOfType<ParallaxLayer>();
        ApplySavedPositionIfAny();
    }

    public void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat(PosX, position.x);
        PlayerPrefs.SetFloat(PosY, position.y);
        PlayerPrefs.SetFloat(PosZ, position.z);
        PlayerPrefs.SetString(SceneKey, SceneManager.GetActiveScene().name);

        Camera cam = Camera.main;
        if (cam != null)
        {
            PlayerPrefs.SetFloat(CamX, cam.transform.position.x);
            PlayerPrefs.SetFloat(CamY, cam.transform.position.y);
            PlayerPrefs.SetFloat(CamZ, cam.transform.position.z);
        }

        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.SaveState();
        }

        PlayerPrefs.Save();
        Debug.Log("Checkpoint Saved");
    }

    public void LoadCheckpoint()
    {
        if (!PlayerPrefs.HasKey(SceneKey))
            return;

        SceneManager.LoadScene(PlayerPrefs.GetString(SceneKey));
    }

    private void ApplySavedPositionIfAny()
    {
        if (!PlayerPrefs.HasKey(PosX))
            return;

        if (SceneManager.GetActiveScene().name != PlayerPrefs.GetString(SceneKey))
            return;

        transform.position = new Vector3(
            PlayerPrefs.GetFloat(PosX),
            PlayerPrefs.GetFloat(PosY),
            PlayerPrefs.GetFloat(PosZ)
        );

        Debug.Log("Checkpoint Loaded");
    }

    public void RemoveSaves()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Saves Deleted");
    }
}
