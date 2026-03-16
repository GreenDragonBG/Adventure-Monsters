using System;
using System.Collections;
using Saves;
using TMPro;
using UnityEngine;

public class InfoScreen : MonoBehaviour
{
    [SerializeField] private string infoID;
    [SerializeField] private bool hasDelay;
    private GameObject _box;
    private TextMeshProUGUI _continueText;
    private Collider2D _collider2D;
    
    void Start()
    {
        if (!OptionsSave.Data.TutorialIsActive)
        {
            SaveSystem.CurrentData.doneInfoScreens.Add(infoID);
        }

        if (SaveSystem.CurrentData.doneInfoScreens.Contains(infoID))
        {
            gameObject.SetActive(false);
        }

        _box = transform.GetChild(0).gameObject;
        _continueText =  transform.GetComponentsInChildren<TextMeshProUGUI>(true)[0];
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _collider2D.enabled = false;
            StartCoroutine(PopUp());
        }
    }
    
    private IEnumerator PopUp()
    {
        if (hasDelay)
        {
            yield return new WaitForSeconds(2.5f);
        }
        _box.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(3.5f);
        
        _continueText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_continueText.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            SaveSystem.CurrentData.doneInfoScreens.Add(infoID);
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
