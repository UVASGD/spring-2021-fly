using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Singleton pattern. Ensure only one player exists at a time.
    public static Player instance;

    public PlayerController playerController;
    public CameraController cameraController;
    public ModelController modelController;

    private void Awake()
    {
        // Singleton, but prefer the newer one to the older one
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }
}
