using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour, ISavable
{
    public ShopButton prereq;
    public GameObject lockedOverlay;
    public GameObject purchasedOverlay;

    public bool locked;
    public bool purchased;

    public bool autoUnlock;

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
    }

    public void TryPurchase()
    {

    }
}
