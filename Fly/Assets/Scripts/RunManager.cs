using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{

    public GameObject canvas;

    private bool restartable;

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
        if (!restartable) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
