using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunManager : MonoBehaviour
{
    private bool alive;

    [Header("References")]
    public GameObject restartUI;
    public UnityEvent OnRestart;

    [Header("Goal Info")]
    public Vector3 goalPosition;
    public Transform goal;
    public float goalRadius = 5f;
    public float goalHeight = 5f;

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
        alive = true;
        if (goal) goalPosition = goal.position;

        GameManager.instance.playerManager.activePlayer.playerController.OnDeath.AddListener(PromptRestart);
    }

    public void PromptRestart()
    {
        restartUI?.SetActive(true);
        alive = false;
    }

    private void Update()
    {
        
        if (alive)
        {
            currentHeight = GameManager.instance.playerManager.activePlayer.transform.position.y;
            currentHeightUI?.SetNumber(currentHeight);

            maxHeightThisRun = Mathf.Max(maxHeightThisRun, currentHeight);
            maxHeightThisRunUI?.SetNumber(maxHeightThisRun);

            distanceFromGoal = (goalPosition - GameManager.instance.playerManager.activePlayer.transform.position).magnitude;
            distanceFromGoalUI?.SetNumber(distanceFromGoal);
            return;
        }
        else
        {

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartRun();
        }
    }

    public void RestartRun()
    {
        OnRestart?.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
