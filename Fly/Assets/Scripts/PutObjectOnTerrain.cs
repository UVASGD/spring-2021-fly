using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutObjectOnTerrain : MonoBehaviour
{
    public Transform tf;
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    // Start is called before the first frame update

    //currently have it updating every frame for testing reasons- will change to only on start when actually doing things

    void Start()
    {
        float meshWorldSize = meshSettings.meshWorldSize;
        float xPos = tf.position.x;
        float zPos = tf.position.z;
        Vector2 thisLocation = new Vector2(xPos, zPos);
        float[,] hm = Noise.GenerateNoiseMap(1,1,heightMapSettings.noiseSettings,thisLocation);
        float height = hm[0, 0];
        AnimationCurve heightCurve_threadsafe = new AnimationCurve(heightMapSettings.heightCurve.keys);
        height *= heightCurve_threadsafe.Evaluate(height) * heightMapSettings.heightMultiplier;
        transform.position = new Vector3(xPos, height, zPos);
    }

    // Update is called once per frame

}

