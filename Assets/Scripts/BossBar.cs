using System;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth = 100;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = maxHealth;
    }

    private void Update()
    {
        slider.value = currentHealth;
    }
}
