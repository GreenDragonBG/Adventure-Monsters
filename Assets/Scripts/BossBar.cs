using System;
using UnityEngine;
using UnityEngine.UI;

public class BossBar : MonoBehaviour
{
    [NonSerialized]public float maxHealth;
    [NonSerialized]public float currentHealth;
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }
}
