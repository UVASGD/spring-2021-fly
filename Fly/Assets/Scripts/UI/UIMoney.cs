using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoney : MonoBehaviour
{
    public UITextNumber textNumber;

    private void Start()
    {
        UpdateMoney();
    }

    public void UpdateMoney()
    {
        textNumber.SetNumber(MoneyManager.instance.money, 0);
    }
}
