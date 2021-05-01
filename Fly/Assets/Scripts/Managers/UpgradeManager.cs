using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour, ISavable
{
    public static UpgradeManager instance;

    public TieredUpgradeList tieredUpgradeList;

    public void Load()
    {
        for (int i = 0; i < tieredUpgradeList.upgrades.Count; i++)
        {
            string key = tieredUpgradeList.upgrades[i].type.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                tieredUpgradeList.upgrades[i].activeTierIndex = PlayerPrefs.GetInt(key);
            }
        }
    }

    public void Save()
    {
        for (int i = 0; i < tieredUpgradeList.upgrades.Count; i++)
        {
            string key = tieredUpgradeList.upgrades[i].type.ToString();
            PlayerPrefs.SetInt(key, tieredUpgradeList.upgrades[i].activeTierIndex);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SetUpgradeTier(TieredUpgrade.Type type, int tier)
    {
        for (int i = 0; i < tieredUpgradeList.upgrades.Count; i++)
        {
            if (tieredUpgradeList.upgrades[i].type == type)
            {
                tieredUpgradeList.upgrades[i].activeTierIndex = tier;
            }
        }
        Save();
    }
}
