using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeManager : MonoBehaviour, ISavable
{
    public static UpgradeManager instance;

    [SerializeField]
    private TieredUpgradeList tieredUpgradeList;

    public Dictionary<TieredUpgrade.Type, int> tieredUpgradeIndexMap { get; private set; }
    public Dictionary<TieredUpgrade.Type, float[]> tieredUpgradeValuesMap { get; private set; }

    public void Load()
    {
        tieredUpgradeIndexMap = new Dictionary<TieredUpgrade.Type, int>();
        tieredUpgradeValuesMap = new Dictionary<TieredUpgrade.Type, float[]>();
        for (int i = 0; i < tieredUpgradeList.upgrades.Count; i++)
        {
            TieredUpgrade.Type type = tieredUpgradeList.upgrades[i].type;
            string key = type.ToString();
            int activeTierIndex = 0;

            if (PlayerPrefs.HasKey(key))
            {
                activeTierIndex = PlayerPrefs.GetInt(key);
            }

            tieredUpgradeIndexMap.Add(type, activeTierIndex);
            tieredUpgradeValuesMap.Add(type, tieredUpgradeList.upgrades[i].tiers.Select(tier => tier.value).ToArray());
        }
    }

    public void Save()
    {
        for (int i = 0; i < tieredUpgradeList.upgrades.Count; i++)
        {
            TieredUpgrade.Type type = tieredUpgradeList.upgrades[i].type;
            string key = type.ToString();
            PlayerPrefs.SetInt(key, tieredUpgradeIndexMap[type]);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Load();
    }

    public void SetUpgradeTier(TieredUpgrade.Type type, int tier)
    {
        tieredUpgradeIndexMap[type] = tier;
        string key = type.ToString();
        PlayerPrefs.SetInt(key, tieredUpgradeIndexMap[type]);
    }

    public int GetUpgradeTier(TieredUpgrade.Type type)
    {
        return tieredUpgradeIndexMap[type];
    }

    public float GetUpgradeValue(TieredUpgrade.Type type)
    {
        return tieredUpgradeValuesMap[type][GetUpgradeTier(type)];
    }
}
