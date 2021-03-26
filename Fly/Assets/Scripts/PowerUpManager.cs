using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public GameObject powerUpObject;
    private int numberOfPowerUpObjects = 500;
    // Start is called before the first frame update
    void Start()
    {
        GenerateObject(powerUpObject, numberOfPowerUpObjects);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //spawn "n" number of GameObject "o"
    private void GenerateObject(GameObject o, int n)
    {
        if (o == null) return;
        for(int i = 0; i < n; i++)
        {
            GameObject tmp = Instantiate(o);

            Vector3 position = getVectorInBounds();
            tmp.gameObject.transform.position = position;
        }
    }

    // Return a Vector3 that's above the floor and below the skyine
    /*
     * IN PROGRESS
     */
    Vector3 getVectorInBounds()
    {
        int xrand = (int)Random.Range(-500, 500);
        int yrand = (int)Random.Range(60, 100);
        int zrand = (int)Random.Range(-500, 500);

        return new Vector3(xrand, yrand, zrand);
    }
}
