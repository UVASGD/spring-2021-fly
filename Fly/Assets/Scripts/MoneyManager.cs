using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour, ISavable
{
    public static MoneyManager instance;

    public float startingMoney;
    private float money;

    public void Load()
    {
        if (PlayerPrefs.HasKey("Money"))
        {
            money = PlayerPrefs.GetFloat("Money");
        }
        else
        {
            money = startingMoney;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
