using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField]private Image heart1;
    [SerializeField]private Image heart2;
    [SerializeField]private Image heart3;
    [SerializeField]private Sprite fullHeart;
    [SerializeField]private Sprite emptyHeart;

    void Update()
    {
        if (Swordman.playerHealth>=90)
        {
            heart1.sprite = fullHeart;
            heart2.sprite = fullHeart;
            heart3.sprite = fullHeart;
        }else if (Swordman.playerHealth < 30)
        {
            heart1.sprite = emptyHeart;
            heart2.sprite = emptyHeart;
            heart3.sprite = emptyHeart;
        }else if (Swordman.playerHealth < 60)
        {
            heart1.sprite = fullHeart;
            heart2.sprite = emptyHeart;
            heart3.sprite = emptyHeart;
        }
        else if (Swordman.playerHealth < 90)
        {
            heart1.sprite = fullHeart;
            heart2.sprite = fullHeart;
            heart3.sprite = emptyHeart;
        }
    }
}
