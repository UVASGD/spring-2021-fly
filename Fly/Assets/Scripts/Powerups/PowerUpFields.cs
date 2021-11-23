using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFields : MonoBehaviour
{
    public float effect;
    public bool persistent;

    public void Use()
    {
        if (!persistent)
        {
            Destroy(gameObject);
        }
    }
}   
