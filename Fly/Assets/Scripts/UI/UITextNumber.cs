using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextNumber : MonoBehaviour
{
    private TMP_Text textField;

    private void Awake()
    {
        textField = GetComponent<TMP_Text>();
    }

    public void SetNumber(float number, int decimalPlaces = 1)
    {
        textField.SetText(number.ToString($"F{decimalPlaces}"));
    }
}
