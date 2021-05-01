using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloatEvent : UnityEvent<float> { }

public class MoneyManager : MonoBehaviour, ISavable
{
    public static MoneyManager instance;

    public FloatEvent OnMoneyChanged;

    public float startingMoney;
    public float money;

    public void Load()
    {
        if (PlayerPrefs.HasKey("Money"))
        {
            money = PlayerPrefs.GetFloat("Money");
        }
        else
        {
            SetMoney(startingMoney);
            Save();
        }
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("Money", money);
    }



    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        OnMoneyChanged = new FloatEvent();

        Load();
    }

    public void SetMoney(float value)
    {
        money = value;
        OnMoneyChanged?.Invoke(value);
    }
}
