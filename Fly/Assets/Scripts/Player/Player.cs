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
}
