using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager instance;
    #endregion

    #region REFERENCES
    public PlayerController playerController;
    public RunManager runManager;
    public TerrainGenerator terrainGenerator;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
