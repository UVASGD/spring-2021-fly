using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopButton : MonoBehaviour, ISavable
{
    public ShopButton successor;
    public TieredUpgrade.Type upgradeType;
    public int upgradeTier;
    public float upgradeCost;
    public GameObject lockedOverlay;
    public GameObject purchasedOverlay;
    public TMP_Text moneyField;

    public bool locked;
    public bool purchased;

    public bool autoUnlock;

    private void Start()
    {
        if (upgradeCost == 0)
        {
            moneyField.SetText("");

        }
        else
        {
            moneyField.SetText(upgradeCost.ToString());
        }
        Load();
    }

    public void Load()
    {
        string lockedKey = $"{gameObject.name}-locked";
        string purchasedKey = $"{gameObject.name}-purchased";

        if (PlayerPrefs.HasKey(lockedKey))
        {
            locked = PlayerPrefs.GetInt(lockedKey) == 0 ? false : true;
        }
        else
        {
            locked = !autoUnlock;
        }

        if (PlayerPrefs.HasKey(purchasedKey))
        {
            purchased = PlayerPrefs.GetInt(purchasedKey) == 0 ? false : true;
        }
        UpdateButton();
    }

    public void Save()
    {
        string lockedKey = $"{gameObject.name}-locked";
        string purchasedKey = $"{gameObject.name}-purchased";
        PlayerPrefs.SetInt(lockedKey, locked ? 1 : 0);
        PlayerPrefs.SetInt(purchasedKey, purchased ? 1 : 0);
    }

    public void UpdateButton()
    {
        if (purchased)
        {
            purchasedOverlay.SetActive(true);
            lockedOverlay.SetActive(false);
        }
        else if (locked)
        {
            purchasedOverlay.SetActive(false);
            lockedOverlay.SetActive(true);
        }
        else
        {
            purchasedOverlay.SetActive(false);
            lockedOverlay.SetActive(false);
        }
        Save();
    }

    public void TryPurchase()
    {
        if (locked || purchased) return;
        float currentMoney = MoneyManager.instance.money;
        if (currentMoney >= upgradeCost)
        {
            MoneyManager.instance.SubtractMoney(upgradeCost);
            UpgradeManager.instance.SetUpgradeTier(upgradeType, upgradeTier);
            purchased = true;
            UpdateButton();
            if (successor)
            {
                successor.locked = false;
                successor.UpdateButton();
            }
        }
    }
}
