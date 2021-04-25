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
    public MapSettings mapSettings;

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
        Vector3 goalPosition = Quaternion.Euler(0f, mapSettings.goalRotationFromForward, 0f) * new Vector3(0f, 0f, mapSettings.goalDistance);
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
}
