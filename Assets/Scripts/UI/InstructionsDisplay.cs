using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InstructionsDisplay : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI continueText;
    private PlayerController player;
    private void Start()
    {
        player= FindAnyObjectByType<PlayerController>();
        continueText.gameObject.SetActive(false);
        player.canMove = false;
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        StartCoroutine(PopUp());
    }
    
    private IEnumerator PopUp()
    {
        yield return new WaitForSeconds(5);
        
        continueText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (continueText.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.SetActive(false);
            player.canMove = true;
        }
    }
}
