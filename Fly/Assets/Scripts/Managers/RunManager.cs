using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunManager : MonoBehaviour
{
    public bool runStarted;
    public bool runEnded;

    [Header("Events")]
    public UnityEvent OnRestart;

    [Header("Goal")]
    public Vector3 goalPosition;
    public Goal goal;
    [SerializeField]
    private GameObject goalPrefab;

    [Header("UI")]
    public Canvas runCanvas;
    public UITextNumber distanceTravelledUI;
    public UITextNumber currentHeightUI;
    public UITextNumber fuelUI;

    [Header("Stats")]
    public float distanceTravelled;
    public float distanceFromGoal;
    public float maxHeightThisRun;
    public float currentHeight;



    private void Start()
    {
        runStarted = false;
        runEnded = false;
        runCanvas?.gameObject.SetActive(false);
    }

    // Always call this before StartRun()
    public void InitRun(Vector3 goalPosition)
    {
        runStarted = false;
        runEnded = false;
        runCanvas?.gameObject.SetActive(true);
        fuelUI?.gameObject.SetActive(UpgradeManager.instance.GetUpgradeTier(TieredUpgrade.Type.RocketScience) > 0);
        goal = Instantiate(goalPrefab, goalPosition, Quaternion.identity).GetComponent<Goal>();
        this.goalPosition = goalPosition;
        GameManager.instance.playerManager.activePlayer.playerController.OnCrash.AddListener(StopRun);
    }

    public void StartRun()
    {
        Player player = GameManager.instance.playerManager.activePlayer;
        Transform parent = player.transform.parent;
        player.transform.SetParent(null);
        player.transform.position = parent.position;
        player.transform.rotation = Quaternion.identity;
        player.cameraController.SetAliveCam();
        player.playerController.SyncUpgrades();
        player.playerController.Init();
        runStarted = true;
    }

    public void StopRun()
    {
        if (runEnded) return;
        runEnded = true;
        runCanvas?.gameObject.SetActive(false);
        MoneyManager.instance.AddMoney(Mathf.Floor(distanceTravelled / 10f));
        SceneManager.instance.LoadScene(1);
    }

    public void CompleteRun()
    {
        if (runEnded) return;
        runEnded = true;
        runCanvas?.gameObject.SetActive(false);
        MoneyManager.instance.AddMoney(Mathf.Floor(distanceTravelled / 10f * 2f));
        SceneManager.instance.LoadScene(0);
    }

    private void Update()
    {
        
        if (runStarted && !runEnded && GameManager.instance.playerManager.activePlayer != null)
        {
            currentHeight = GameManager.instance.playerManager.activePlayer.transform.position.y;
            currentHeightUI?.SetNumber(currentHeight, 0);

            distanceTravelled = new Vector3(GameManager.instance.playerManager.activePlayer.transform.position.x,
                0f,
                GameManager.instance.playerManager.activePlayer.transform.position.z).magnitude;
            distanceTravelledUI?.SetNumber(distanceTravelled, 0);

            fuelUI?.SetNumber(GameManager.instance.playerManager.activePlayer.playerController.fuel, 0);


            float meshWorldSize = GameManager.instance.currentTerrainGenerator.meshSettings.meshWorldSize;
            float xPos = GameManager.instance.playerManager.activePlayer.transform.position.x;
            float zPos = GameManager.instance.playerManager.activePlayer.transform.position.z;

            Vector2 thisLocation = new Vector2(xPos, zPos);
            float[,] hm = Noise.GenerateNoiseMap(1, 1,
                GameManager.instance.currentTerrainGenerator.heightMapSettings.noiseSettings,
                thisLocation);
            float height = hm[0, 0];
            AnimationCurve heightCurve_threadsafe = new AnimationCurve(GameManager.instance.currentTerrainGenerator.heightMapSettings.heightCurve.keys);
            height *= heightCurve_threadsafe.Evaluate(height) * GameManager.instance.currentTerrainGenerator.heightMapSettings.heightMultiplier;

            if (GameManager.instance.playerManager.activePlayer.transform.position.y < height)
            {
                StopRun();
            }
        }

        
    }
}
