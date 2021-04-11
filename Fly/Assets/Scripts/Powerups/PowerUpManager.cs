using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class PowerUp
{
    public int spawnDensityPerChunk; // How many should spawn per chunk?
    public GameObject prefab; // Select the proper prefab from the Assets folder
}

public class PowerUpManager : MonoBehaviour
{
    

    public List<GameObject> powerUpObjects;
    private int maxNumberOfPowerUpObjects = 6; //max number of powerups in each chunk
    private int[] probabilitiesToSpawn = {5, 5, 5}; //this should be out of 100

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public List<GameObject> GenerateObject(Bounds b)
    {
        return GenerateObject(powerUpObjects, maxNumberOfPowerUpObjects, b);
    }

    //spawn "n" number of GameObject "o"
    private List<GameObject> GenerateObject(List<GameObject> o, int n, Bounds bounds)
    {
        List<GameObject> gameObjectsList = new List<GameObject>();
        if (o == null) return gameObjectsList;
        for(int i = 0; i < n; i++)
        {
            int spawnObjectIndex = (int)Random.Range(0, powerUpObjects.Count);
            int spawnOrNoSpawn = (int)Random.Range(0, 100);
            
            if(spawnOrNoSpawn < probabilitiesToSpawn[spawnObjectIndex])
            {
                GameObject tmp = Instantiate(powerUpObjects[spawnObjectIndex]);
                gameObjectsList.Add(tmp);
                Vector3 position = getVectorInBounds(bounds);
                tmp.gameObject.transform.position = position;
                //Debug.Log("Made at position: " + position);
            }
            
        }
        return gameObjectsList;
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
