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
    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        OnMoneyChanged = new FloatEvent();

        Load();

        if (GameManager.instance.debugMode)
        {
            SetMoney(999999f);
        }
    }

    public void SetMoney(float value)
    {
        money = Mathf.Max(0f, value);
        OnMoneyChanged?.Invoke(value);
    }

    public void AddMoney(float value)
    {
        SetMoney(money + value);
    }

    public void SubtractMoney(float value)
    {
        SetMoney(money - value);
    }
}
