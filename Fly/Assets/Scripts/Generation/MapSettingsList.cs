using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// UPDATE ME WHEN YOU MAKE A NEW MAP
public enum MapType
{
    LushMountainRange,
    Desert,
}

[System.Serializable]
public class MapSettings : ISavable
{
    [Header("General")]
    public string name;
    public Sprite image;
    public MapType map;

    public bool locked;
    public bool completed;

    [Scene]
    public string scene;


    [Header("Goal")]
    // Store goal location in polar coordinates
    public float goalDistance;
    public float goalRotationFromForward;

    public void Load()
    {
        if (PlayerPrefs.HasKey("map_" + name + "_locked"))
        {
            locked = PlayerPrefs.GetInt("map_" + name + "_locked") > 0 ? true : false;
        }
        if (PlayerPrefs.HasKey("map_" + name + "_completed"))
        {
            locked = PlayerPrefs.GetInt("map_" + name + "_completed") > 0 ? true : false;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("map_" + name + "_locked", locked ? 1 : 0);
        PlayerPrefs.SetInt("map_" + name + "_completed", completed ? 1 : 0);
    }
}


[CreateAssetMenu()]
public class MapSettingsList : ScriptableObject
{
    public List<MapSettings> mapSettingsList;
    public Dictionary<MapType, MapSettings> mapsDict; // Used for constant time settings look-up

    public void UpdateDictionary()
    {
        if (mapsDict == null)
        {
            mapsDict = new Dictionary<MapType, MapSettings>();
        }
        else
        {
            mapsDict.Clear();
        }

        foreach (var mapSettings in mapSettingsList)
        {
            if (!mapsDict.ContainsKey(mapSettings.map))
            {
                mapsDict.Add(mapSettings.map, mapSettings);
            }
        }
    }
}