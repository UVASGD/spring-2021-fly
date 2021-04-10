using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera aliveCamera;
    public CinemachineVirtualCamera deadCamera;

    private void Start()
    {
        GameManager.instance.runManager.OnRestart.AddListener(SetAliveCam);
    }

    public void SetDeadCam()
    {
        aliveCamera.gameObject.SetActive(false);
        deadCamera.gameObject.SetActive(true);
    }

    public void SetAliveCam()
    {
        aliveCamera.gameObject.SetActive(true);
        deadCamera.gameObject.SetActive(false);
    }
}
