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
        while (SceneManager.instance.transitioning)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        InitLevel();
    }

    public void InitLevel()
    {
        Player player = playerManager.SpawnPlayerAtPosition(new Vector3(0f, 50f, 0f));
        player.modelController.Init();
        player.modelController.SyncActiveModel();
        FindObjectOfType<TerrainGenerator>().viewer = player.transform;

        // Get goal position from the map settings polar coordinates
        Vector3 goalPosition = Quaternion.Euler(0f, currentMapSettings.goalRotationFromForward, 0f) * new Vector3(0f, 0f, currentMapSettings.goalDistance);
        runManager.InitRun(goalPosition);
    }

    public void StartLevel()
    {
        runManager.StartRun();
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
        print("ALl LEVELS COMPLETED!");
    }
}
