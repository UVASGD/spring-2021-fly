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

    private void Update()
    {
        if (!debugMode) return;
        if (sceneManager.currentSceneBuildIndex == 0) return; // Can't start game on title screen
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
        }
    }

    public void LoadLevel()
    {

    }

    public void InitLevel()
    {
        Player player = playerManager.SpawnPlayerAtPosition(new Vector3(0f, 50f, 0f));

    }

    public void StartLevel()
    {

        // Get goal position from the map settings polar coordinates
        Vector3 goalPosition = Quaternion.Euler(0f, mapSettings.goalRotationFromForward, 0f) * new Vector3(0f, 0f, mapSettings.goalDistance);
        runManager.InitRun(goalPosition);
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
