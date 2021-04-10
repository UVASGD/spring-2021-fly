using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFields : MonoBehaviour
{
    private string powerUpName;
    public int effect;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    string getPowerUpName()
    {
        return this.powerUpName;
    }
    int getEffect()
    {
        return this.effect;
    }
}   
