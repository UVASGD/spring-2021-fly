using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class MapSettings : ISavable
{
    [Header("General")]
    public string name;
    public Sprite image;

    public bool locked;
    public bool completed;

    public GameObject character;

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