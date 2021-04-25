using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILevelSelector : MonoBehaviour, ISavable
{
    public MapSettingsList mapList;
    public GameObject UILevelPrefab;

    public int levelCount;
    public int currentIndex;
    public List<MapSettings> mapSettingsList;
    public List<GameObject> mapUIList;

    // Start is called before the first frame update
    void Start()
    {
        currentIndex = 0;
        levelCount = 0;
        int unlockedCount = 0;

        mapSettingsList = new List<MapSettings>();
        mapUIList = new List<GameObject>();

        foreach (var item in mapList.mapSettingsList)
        {
            levelCount++;
            GameObject element = Instantiate(UILevelPrefab, transform);
            Image image = element.GetComponentInChildren<Image>();
            TMP_Text text = element.GetComponentInChildren<TMP_Text>();

            if (item.image != null)
            {
                image.sprite = item.image;
            }

            if (item.name != "")
            {
                text.SetText(item.name);
            }
            else
            {
                text.SetText("Level " + levelCount);
            }

            mapSettingsList.Add(item);
            mapUIList.Add(element);

            element.SetActive(levelCount == 1);
        }

        // If no maps unlocked, unlock the first one
        if (unlockedCount == 0)
        {
            mapSettingsList[0].locked = false;
            mapSettingsList[0].Save();
            mapUIList[0].transform.Find("LockPanel").gameObject.SetActive(false);
        }
    }

    public void NextLevel()
    {
        mapUIList[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % levelCount;
        mapUIList[currentIndex].SetActive(true);
    }

    public void PreviousLevel()
    {
        mapUIList[currentIndex].SetActive(false);
        currentIndex = (currentIndex - 1) + (currentIndex > 0 ? 0 : levelCount);
        mapUIList[currentIndex].SetActive(true);
    }

    public void ConfirmLevel()
    {
        MapSettings settings = mapSettingsList[currentIndex];
        if (settings.locked)
        {
            OnInvalidLevel();
            return;
        }
        GameManager.instance.mapSettings = settings;
        SceneManager.instance.LoadScene(settings.scene);
    }

    public void OnInvalidLevel()
    {
        print("INVALID LEVEL");
    }

    public void Save()
    {
        foreach (var item in mapSettingsList)
        {
            item.Save();
        }
    }

    public void Load()
    {
        foreach (var item in mapSettingsList)
        {
            item.Load();
        }
    }
}
