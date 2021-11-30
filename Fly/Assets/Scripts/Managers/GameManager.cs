using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager instance;
    #endregion

    #region REFERENCES
    public PlayerManager playerManager;
    public RunManager runManager;
    public SceneManager sceneManager;
    public MoneyManager moneyManager;
    public UpgradeManager upgradeManager;
    #endregion

    [HideInInspector]
    public List<MapSettings> mapSettingsList;
    [HideInInspector]
    public MapSettings currentMapSettings;
    public TerrainGenerator currentTerrainGenerator { get; private set; }

    public bool debugMode;

    // Start is called before the first frame update
    private void Awake()
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

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    public void LoadLevel()
    {
        LoadLevel(currentMapSettings.scene);
    }

    public void LoadLevel(string name)
    {
        sceneManager.LoadScene(name);
    }

    public void InitLevel(TerrainGenerator terrainGenerator)
    {
        currentTerrainGenerator = terrainGenerator;

        GameObject thrower = Instantiate(currentMapSettings.character, Vector3.up * 100f, Quaternion.identity);
        PutObjectOnTerrain putter = thrower.GetComponent<PutObjectOnTerrain>();
        putter.SnapToTerrain(terrainGenerator.meshSettings, terrainGenerator.heightMapSettings);

        GameObject throwPoint = GameObject.FindWithTag("ThrowPoint");

        Player player = playerManager.SpawnPlayerAtObject(throwPoint);
        player.transform.position = throwPoint.transform.position;
        player.transform.rotation = throwPoint.transform.rotation;
        player.transform.SetParent(throwPoint.transform);
        player.cameraController.SetThrowCam(thrower.transform);
        player.modelController.Init();
        player.modelController.SyncActiveModel();

        terrainGenerator.Init(player.transform);

        // Get goal position from the map settings polar coordinates
        Vector3 goalPosition = Quaternion.Euler(0f, currentMapSettings.goalRotationFromForward, 0f) * new Vector3(0f, 0f, currentMapSettings.goalDistance);
        runManager.InitRun(goalPosition);

        if (currentMapSettings.landmark != null)
        {
            GameObject landmark = Instantiate(currentMapSettings.landmark);
            if (landmark.GetComponent<PutObjectOnTerrain>() == null)
            {
                landmark.AddComponent<PutObjectOnTerrain>();
            }
            Vector3 goalDirection = goalPosition.normalized;
            Vector2 offset = Random.insideUnitCircle * 50f;
            landmark.transform.position = goalPosition + goalDirection * 300f + new Vector3(offset.x, 0f, offset.y);
            landmark.GetComponent<PutObjectOnTerrain>().SnapToTerrain(terrainGenerator.meshSettings, terrainGenerator.heightMapSettings);
        }

        Invoke("StartLevel", 3f);
    }

    public void StartLevel()
    {
        FindObjectOfType<Thrower>().Throw();
        runManager.Invoke("StartRun", 1f);
    }

    public void Save()
    {
        foreach (var item in FindObjectsOfType<MonoBehaviour>().OfType<ISavable>())
        {
            item.Save();
        }
    }

    public void Load()
    {
        foreach (var item in FindObjectsOfType<MonoBehaviour>().OfType<ISavable>())
        {
            item.Load();
        }
    }

    public void UnlockNextLevel()
    {
        int currentIndex = mapSettingsList.IndexOf(currentMapSettings);
        mapSettingsList[currentIndex].completed = true;
        mapSettingsList[currentIndex].Save();

        if (currentIndex < mapSettingsList.Count - 1)
        {
            mapSettingsList[currentIndex + 1].locked = false;
            mapSettingsList[currentIndex + 1].Save();
        }
        else
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        print("ALL LEVELS COMPLETED!");
    }
}
