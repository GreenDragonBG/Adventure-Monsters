using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ParallaxLayerEnd : MonoBehaviour
{
    [SerializeField] private ParallaxLayer parallaxLayer;
    [SerializeField] private bool comingFromLeft;
    
    [SerializeField]private float timeToFadeOut;
    [SerializeField]private bool startFadedOut;
    private Coroutine currentFade;
    private SpriteRenderer sprite;

    private float rememberedFactor;

    private void Start()
    {
        rememberedFactor = parallaxLayer.parallaxFactor;
        
        sprite = parallaxLayer.gameObject.GetComponent<SpriteRenderer>();
        if (startFadedOut)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        bool isLeft = (transform.position.x > other.transform.position.x);
        
        if (comingFromLeft)
        {
            parallaxLayer.parallaxFactor = !isLeft ? rememberedFactor :  0f;
            StartFade(isLeft ? FadeOut() : FadeIn());
        }
        else
        {
            parallaxLayer.parallaxFactor = isLeft ? rememberedFactor :  0f;
            StartFade(!isLeft ? FadeOut() : FadeIn());
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
        while (sprite.color.a > 0f)
        {
            t += Time.deltaTime / timeToFadeOut;
            float value = Mathf.Lerp(sprite.color.a, 0f, t);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, value);
            yield return null;
        }
    }
    
    private IEnumerator FadeIn()
    {
        float t = 0f;
        while (sprite.color.a < 1f)
        {
            t += Time.deltaTime / timeToFadeOut;
            float value = Mathf.Lerp(sprite.color.a, 1f, t);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, value);
            yield return null;
        }
    }
}