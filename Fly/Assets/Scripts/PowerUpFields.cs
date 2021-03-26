using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFields : MonoBehaviour
{
    private string objectType;
    private int effect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void InitializeFields(string ObjectType, int Effect)
    {
        this.objectType = ObjectType;
        this.effect = Effect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    string getObjectType()
    {
        return this.objectType;
    }
    int getEffect()
    {
        return this.effect;
    }
}   
