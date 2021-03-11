using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    private bool restartable;

    [Header("References")]
    public GameObject canvas;

    [Header("Goal Info")]
    public Vector3 goalPosition;
    public float goalRadius = 5f;
    public float goalHeight = 5f;

    [Header("Run Stats")]
    public float distanceTravelled;
    public float distanceFromGoal;
    public float maxHeightThisRun;
    public float currentHeight;


    private void Start()
    {
        canvas?.SetActive(false);
        restartable = false;

        GameManager.instance.playerController.DieEvent.AddListener(PromptRestart);
    }

    public void PromptRestart()
    {
        canvas?.SetActive(true);
        restartable = true;
    }

    private void Update()
    {
        currentHeight = GameManager.instance.playerController.transform.position.y;
        maxHeightThisRun = Mathf.Max(maxHeightThisRun, currentHeight);

        if (!restartable) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
