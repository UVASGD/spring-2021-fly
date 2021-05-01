using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoney : MonoBehaviour
{
    public UITextNumber textNumber;

    private void Start()
    {
        MoneyManager.instance.OnMoneyChanged.AddListener(UpdateMoney);
        UpdateMoney();
    }

    public void UpdateMoney()
    {
        textNumber.SetNumber(MoneyManager.instance.money, 0);
    }

    public void UpdateMoney(float value)
    {
        textNumber.SetNumber(value, 0);
    }
}
