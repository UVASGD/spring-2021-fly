using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UPDATE ME WHEN YOU MAKE A NEW MAP
public enum MapType
{
    LushMountainRange,
    Desert,
}

[System.Serializable]
public class MapSettings
{
    public string name;
    public Sprite image;
    public MapType map;

    [Header("Generation Settings")]
    public HeightMapSettings heightMapSettings;
    public MeshSettings meshSettings;
    public TextureData textureData;

    [Header("Goal Settings")]
    // Store goal location in polar coordinates
    public float goalDistance;
    public float goalRotationFromForward;
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
            mapsDict.Add(mapSettings.map, mapSettings);
        }
    }
}