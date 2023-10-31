using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textNumber;

    public TextMeshProUGUI GetNumber()
    {
        return textNumber;
    }
    public void SetNumber(int num)
    {
        textNumber.text = num.ToString();
    }
}
