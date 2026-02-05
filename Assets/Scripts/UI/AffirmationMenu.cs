using System;
using UnityEngine;

public class AffirmationMenu : MonoBehaviour
{
    public int result;

    public void YesButton()
    {
        result = 1;
    }

    public void NoButton()
    {
        result = 0;
    }
}
