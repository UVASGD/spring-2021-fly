using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunManager : MonoBehaviour
{
    private bool runStarted;
    private bool runEnded;

    [Header("References")]
    public GameObject restartUI;
    public UnityEvent OnRestart;

    [Header("Goal Info")]
    public Vector3 goalPosition;
    public Goal goal;
    [SerializeField]
    private GameObject goalPrefab;

    [Header("Run Stats")]
    public float distanceTravelled;
    public UITextNumber distanceTravelledUI;
    public float distanceFromGoal;
    public UITextNumber distanceFromGoalUI;
    public float maxHeightThisRun;
    public UITextNumber maxHeightThisRunUI;
    public float currentHeight;
    public UITextNumber currentHeightUI;

    private void Start()
    {
        restartUI?.SetActive(false);
        runStarted = false;
        runEnded = false;
    }

    // Always call this before StartRun()
    public void InitRun(Vector3 goalPosition)
    {
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
    }

    private void Update()
    {
        
        if (runStarted && !runEnded)
        {
            currentHeight = GameManager.instance.playerManager.activePlayer.transform.position.y;
            currentHeightUI?.SetNumber(currentHeight);

            maxHeightThisRun = Mathf.Max(maxHeightThisRun, currentHeight);
            maxHeightThisRunUI?.SetNumber(maxHeightThisRun);

            distanceFromGoal = (goalPosition - GameManager.instance.playerManager.activePlayer.transform.position).magnitude;
            distanceFromGoalUI?.SetNumber(distanceFromGoal);
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
