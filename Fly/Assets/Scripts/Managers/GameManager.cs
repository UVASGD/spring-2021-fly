using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    #endregion

    [HideInInspector]
    public List<MapSettings> mapSettingsList;
    [HideInInspector]
    public MapSettings currentMapSettings;
    [HideInInspector]
    public TerrainGenerator currentTerrainGenerator;

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

    public void LoadLevel(string name)
    {
        if (SceneManager.instance.transitioning) return;
        SceneManager.instance.LoadScene(name);
        StartCoroutine(LoadLevelCR());
    }

    private IEnumerator LoadLevelCR()
    {
        yield return new WaitForEndOfFrame();
        while (!SceneManager.instance.sceneLoaded && SceneManager.instance.transitioning)
        {
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        InitLevel();
    }

    public void InitLevel()
    {
        currentTerrainGenerator = FindObjectOfType<TerrainGenerator>();
        GameObject thrower = Instantiate(currentMapSettings.character, Vector3.up * 100f, Quaternion.identity);
        PutObjectOnTerrain putter = thrower.GetComponent<PutObjectOnTerrain>();
        putter.heightMapSettings = currentTerrainGenerator.heightMapSettings;
        putter.meshSettings = currentTerrainGenerator.meshSettings;
        putter.SnapToTerrain();

        GameObject throwPoint = GameObject.FindWithTag("ThrowPoint");

        Player player = playerManager.SpawnPlayerAtObject(throwPoint);
        player.transform.position = throwPoint.transform.position;
        player.transform.rotation = throwPoint.transform.rotation;
        player.transform.SetParent(throwPoint.transform);
        player.cameraController.SetThrowCam(thrower.transform);
        player.modelController.Init();
        player.modelController.SyncActiveModel();
        currentTerrainGenerator.viewer = player.transform;

        // Get goal position from the map settings polar coordinates
        Vector3 goalPosition = Quaternion.Euler(0f, currentMapSettings.goalRotationFromForward, 0f) * new Vector3(0f, 0f, currentMapSettings.goalDistance);
        runManager.InitRun(goalPosition);
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
