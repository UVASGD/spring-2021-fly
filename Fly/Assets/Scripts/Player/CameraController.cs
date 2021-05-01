using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera throwCamera;
    public CinemachineVirtualCamera aliveCamera;
    public CinemachineVirtualCamera deadCamera;

    private void Start()
    {
        //GameManager.instance.runManager.OnRestart.AddListener(SetAliveCam);
    }

    public void SetThrowCam(Transform target)
    {
        throwCamera.Follow = target;
        throwCamera.LookAt = target;
        throwCamera.gameObject.SetActive(true);
        aliveCamera.gameObject.SetActive(false);
        deadCamera.gameObject.SetActive(false);
    }

    public void SetDeadCam()
    {
        throwCamera.gameObject.SetActive(false);
        aliveCamera.gameObject.SetActive(false);
        deadCamera.gameObject.SetActive(true);
    }

    public void SetAliveCam()
    {
        throwCamera.gameObject.SetActive(false);
        aliveCamera.gameObject.SetActive(true);
        deadCamera.gameObject.SetActive(false);
    }
}
