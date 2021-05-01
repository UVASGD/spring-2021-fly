using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour, ISavable
{
    public static UpgradeManager instance;

    public TieredUpgradeList tieredUpgradeList;

    public void Load()
    {
        foreach (var item in tieredUpgradeList.upgrades)
        {
            string key = item.type.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                
            }
        }
    }

    public void Save()
    {

    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
