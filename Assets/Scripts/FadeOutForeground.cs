using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadeOutForeground : MonoBehaviour
{
    [SerializeField]private float timeToFadeOut;
    [SerializeField]private bool startFadedOut;
    private Coroutine currentFade;
    private Tilemap tilemap;
    
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if (startFadedOut)
        {
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(FadeOut());
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(FadeIn());
        }
    }
    
    private void StartFade(IEnumerator fadeRoutine)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(fadeRoutine);
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (tilemap.color.a > 0f)
        {
            t += Time.deltaTime / timeToFadeOut;
            float value = Mathf.Lerp(tilemap.color.a, 0f, t);
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, value);
            yield return null;
        }
    }
    
    private IEnumerator FadeIn()
    {
        float t = 0f;
        while (tilemap.color.a < 1f)
        {
            t += Time.deltaTime / timeToFadeOut;
            float value = Mathf.Lerp(tilemap.color.a, 1f, t);
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, value);
            yield return null;
        }
    }
}
