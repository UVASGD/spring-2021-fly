using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapSettingsList mapSettingsList;
    public TerrainGenerator terrainGenerator;

    private void Awake()
    {
        mapSettingsList.UpdateDictionary();
    }

    // Spawn the player before you call this and pass it in as the viewer
    public MapSettings GenerateMap(MapSettings settings, Transform viewer)
    {
        terrainGenerator.meshSettings = settings.meshSettings;
        terrainGenerator.heightMapSettings = settings.heightMapSettings;
        terrainGenerator.textureSettings = settings.textureData;
        terrainGenerator.viewer = viewer;
        return settings;
    }
}
