using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MapSettings
{
    public string name;
    public Sprite image;
    
    [Scene]
    public string scene;
    public bool locked;
    public bool completed;

    [Header("Goal Settings")]
    // Store goal location in polar coordinates
    public float goalDistance;
    public float goalRotationFromForward;

    public void Load()
    {
        if (PlayerPrefs.HasKey("map_" + name + "_locked"))
        {
            locked = PlayerPrefs.GetInt("map_" + name + "_locked") > 0 ? true : false;
        }
        else
        {
            locked = true;
        }
        if (PlayerPrefs.HasKey("map_" + name + "_completed"))
        {
            completed = PlayerPrefs.GetInt("map_" + name + "_completed") > 0 ? true : false;
        }
        else
        {
            completed = false;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("map_" + name + "_locked", locked ? 1 : 0);
        PlayerPrefs.SetInt("map_" + name + "_completed", completed ? 1 : 0);
    }
}


[CreateAssetMenu(fileName = "Map Settings List", menuName = "ScriptableObjects/Map Settings List")]
public class MapSettingsList : ScriptableObject
{
    public List<MapSettings> mapSettingsList;
}