using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunManager : MonoBehaviour
{
    private bool runStarted;
    private bool runEnded;

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
    public UITextNumber distanceFromGoalUI;
    public UITextNumber maxHeightThisRunUI;
    public UITextNumber currentHeightUI;
    public GameObject restartUI;
    public UITextNumber fuelUI;

    [Header("Stats")]
    public float distanceTravelled;
    public float distanceFromGoal;
    public float maxHeightThisRun;
    public float currentHeight;

    private void Start()
    {
        restartUI?.SetActive(false);
        runStarted = false;
        runEnded = false;
        runCanvas?.gameObject.SetActive(false);
    }

    // Always call this before StartRun()
    public void InitRun(Vector3 goalPosition)
    {
        runCanvas?.gameObject.SetActive(true);
        goal = Instantiate(goalPrefab, goalPosition, Quaternion.identity).GetComponent<Goal>();
        this.goalPosition = goalPosition;
        GameManager.instance.playerManager.activePlayer.playerController.OnDeath.AddListener(StopRun);
    }

    public void StartRun()
    {
        runStarted = true;
    }

    public void StopRun()
    {
        runEnded = true;
        restartUI?.SetActive(true);
        runCanvas?.gameObject.SetActive(false);
    }

    private void Update()
    {
        
        if (runStarted && !runEnded && GameManager.instance.playerManager.activePlayer != null)
        {
            currentHeight = GameManager.instance.playerManager.activePlayer.transform.position.y;
            currentHeightUI?.SetNumber(currentHeight);

            maxHeightThisRun = Mathf.Max(maxHeightThisRun, currentHeight);
            maxHeightThisRunUI?.SetNumber(maxHeightThisRun);

            distanceFromGoal = (goalPosition - GameManager.instance.playerManager.activePlayer.transform.position).magnitude;
            distanceFromGoalUI?.SetNumber(distanceFromGoal);

            fuelUI?.SetNumber(GameManager.instance.playerManager.activePlayer.playerController.fuel);

            return;
        }
        
        if (runEnded)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
