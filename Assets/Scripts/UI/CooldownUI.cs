using System;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    private Image img;
    public float cooldownTime;
    private float timeCooldownStarted;
    private bool isGameStart= true;
    void Start()
    {
        img = GetComponent<Image>();
        isGameStart =false;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (isGameStart) return;
        timeCooldownStarted= Time.time;
        img.fillAmount = 1;
    }

    void Update()
    {
        img.fillAmount =(1 - (Time.time-timeCooldownStarted)/cooldownTime);
        if (img.fillAmount <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
