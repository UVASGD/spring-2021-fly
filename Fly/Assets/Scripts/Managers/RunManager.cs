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
        goal = Instantiate(goalPrefab, goalPosition, Quaternion.identity).GetComponent<Goal>();
        this.goalPosition = goalPosition;
        GameManager.instance.playerManager.activePlayer.playerController.OnDeath.AddListener(StopRun);
    }

    public void StartRun()
    {
        Player player = GameManager.instance.playerManager.activePlayer;
        Transform parent = player.transform.parent;
        player.transform.SetParent(null);
        player.transform.position = parent.position;
        player.transform.rotation = Quaternion.identity;
        player.cameraController.SetAliveCam();
        runStarted = true;
    }

    public void StopRun()
    {
        runEnded = true;
        runCanvas?.gameObject.SetActive(false);
        MoneyManager.instance.AddMoney(Mathf.Floor(distanceTravelled / 100f));
        SceneManager.instance.LoadScene(1);
    }

    public void CompleteRun()
    {
        runEnded = true;
        runCanvas?.gameObject.SetActive(false);
        MoneyManager.instance.AddMoney(Mathf.Floor(distanceTravelled / 100f * 2f));
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

            fuelUI?.SetNumber(GameManager.instance.playerManager.activePlayer.playerController.fuel);

            return;
        }
    }
}
