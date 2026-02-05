using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InstructionsDisplay : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI continueText;
    private void Start()
    {
        continueText.gameObject.SetActive(false);
        Time.timeScale = 0f;
        StartCoroutine(PopUp());
    }
    
    private IEnumerator PopUp()
    {
        yield return new WaitForSecondsRealtime(5);
        
        continueText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (continueText.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
