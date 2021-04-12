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
    public SceneManager sceneManager;
    #endregion

    [SerializeField]
    private bool debugMode;

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
        if (!debugMode) return;
        if (sceneManager.currentSceneBuildIndex == 0) return; // Can't start game on title screen
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartLevel(MapType.Desert, Model.Type.Classic);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartLevel(MapType.Desert, Model.Type.BigFlat);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartLevel(MapType.Desert, Model.Type.Needlenose);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartLevel(MapType.Desert, Model.Type.Stingray);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartLevel(MapType.Desert, Model.Type.Cobra);
        }
    }

    public void StartLevel(MapType map, Model.Type playerModel)
    {
        Player player = playerManager.SpawnPlayerAtPosition(new Vector3(0f, 50f, 0f), playerModel);
        MapSettings settings = mapManager.GenerateMap(mapManager.mapSettingsList.mapsDict[map], player.transform);

        // Get goal position from the map settings polar coordinates
        Vector3 goalPosition = Quaternion.Euler(0f, settings.goalRotationFromForward, 0f) * new Vector3(0f, 0f, settings.goalDistance);
        runManager.InitRun(goalPosition);
        runManager.StartRun();
    }
}
