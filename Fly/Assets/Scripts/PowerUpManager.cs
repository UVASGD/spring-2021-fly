using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public GameObject[] powerUpObjects = new GameObject[2];
    private int maxNumberOfPowerUpObjects = 4; //max number of powerups in each chunk
    private int[] probabilitiesToSpawn = {25, 25, 25}; //this should be out of 100

    // Start is called before the first frame update
    void Start()
    {
        powerUpObjects = GameObject.FindGameObjectsWithTag("PowerUp");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GenerateObject(Bounds b)
    {
        GenerateObject(powerUpObjects, maxNumberOfPowerUpObjects, b);
    }

    //spawn "n" number of GameObject "o"
    private void GenerateObject(GameObject[] o, int n, Bounds bounds)
    {
        if (o == null) return;
        for(int i = 0; i < n; i++)
        {
            int spawnObjectIndex = (int)Random.Range(0, powerUpObjects.Length);
            int spawnOrNoSpawn = (int)Random.Range(0, 100);
            
            if(spawnOrNoSpawn < probabilitiesToSpawn[spawnObjectIndex])
            {
                GameObject tmp = Instantiate(powerUpObjects[spawnObjectIndex]);

                Vector3 position = getVectorInBounds(bounds);
                tmp.gameObject.transform.position = position;
                //Debug.Log("Made at position: " + position);
            }
            
        }
    }

    // Return a Vector3 that's above the floor and below the skyine
    /*
     * IN PROGRESS
     */
    Vector3 getVectorInBounds(Bounds bounds)
    {
        int xrand = (int)Random.Range(bounds.min.x, bounds.max.x);
        int yrand = (int)Random.Range(40, 80);
        int zrand = (int)Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(xrand, yrand, zrand);
    }
}
