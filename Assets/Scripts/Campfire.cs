using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Campfire : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string campfireID;

    private Transform fire;
    private Light2D fireLight;

    [SerializeField] private float scaleGrowth = 0.001f;
    [SerializeField] private float timeInterval = 0.1f;

    void Start()
    {
        fireLight = GetComponentInChildren<Light2D>();
        fire = fireLight.transform.parent;

        fire.localScale = new Vector3(0.1f, 0.1f, 1f);
        fireLight.enabled = false;

        LoadState();
    }

    private void LoadState()
    {
        if (PlayerPrefs.GetInt("Campfire_" + campfireID, 0) == 1)
        {
            fireLight.enabled = true;
            fire.localScale = Vector3.one * 3f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Input.GetKey(KeyCode.C))
        {
            ActivateCampfire(other);
        }
    }

    private void ActivateCampfire(Collider2D player)
    {
        fireLight.enabled = true;

        PlayerPrefs.SetInt("Campfire_" + campfireID, 1);
        player.GetComponent<PlayerController>().playerHealth = 90;
        player.GetComponent<PlayerSave>().SaveCheckpoint(
            new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                player.transform.position.z
            )
        );

        StartCoroutine(GrowFire());
    }

    private IEnumerator GrowFire()
    {
        while (fire.localScale.x < 3f)
        {
            fire.localScale += Vector3.one * scaleGrowth;
            yield return new WaitForSeconds(timeInterval);
        }
    }
}