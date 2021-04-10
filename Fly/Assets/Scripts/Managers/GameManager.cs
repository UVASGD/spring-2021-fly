using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager instance;
    #endregion

    #region REFERENCES
    public PlayerManager playerManager;
    public RunManager runManager;
    public MapManager mapManager;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartLevel(MapType.LushMountainRange);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartLevel(MapType.Desert);
        }
    }

    public void StartLevel(MapType map)
    {
        Player player = playerManager.SpawnPlayerAtPosition(new Vector3(0f, 50f, 0f));
        MapSettings settings = mapManager.GenerateMap(mapManager.mapSettingsList.mapsDict[map], player.transform);

        // Get goal position from the map settings polar coordinates
        Vector3 goalPosition = Quaternion.Euler(0f, settings.goalRotationFromForward, 0f) * new Vector3(0f, 0f, settings.goalDistance);
        runManager.InitRun(goalPosition);
        runManager.StartRun();
    }
}
