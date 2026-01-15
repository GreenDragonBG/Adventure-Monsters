using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string layerID;

    [Header("Parallax")]
    [SerializeField] public float parallaxFactor;

    private void Awake()
    {
        if (!Application.isPlaying)
            return;

        LoadState();
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;
        transform.localPosition = newPos;
    }

    public void SaveState()
    {
        if (!Application.isPlaying)
            return;

        PlayerPrefs.SetFloat("ParallaxLayerX_" + layerID, transform.localPosition.x);
        PlayerPrefs.SetFloat("ParallaxLayerY_" + layerID, transform.localPosition.y);
        PlayerPrefs.SetFloat("ParallaxLayerZ_" + layerID, transform.localPosition.z);
    }

    private void LoadState()
    {
        string keyX = "ParallaxLayerX_" + layerID;
        if (!PlayerPrefs.HasKey(keyX))
            return;

        transform.localPosition = new Vector3(
            PlayerPrefs.GetFloat(keyX),
            PlayerPrefs.GetFloat("ParallaxLayerY_" + layerID),
            PlayerPrefs.GetFloat("ParallaxLayerZ_" + layerID)
        );
    }
}