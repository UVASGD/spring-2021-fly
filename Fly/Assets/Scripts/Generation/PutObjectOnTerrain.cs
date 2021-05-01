using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutObjectOnTerrain : MonoBehaviour
{
    public void SnapToTerrain(MeshSettings meshSettings, HeightMapSettings heightMapSettings)
    {
        //RaycastHit[] hits;
        //if ((hits = Physics.RaycastAll(new Ray(Vector3.up * 1000f, Vector3.down * 2000f))) != null){
        //    foreach (var hit in hits)
        //    {
        //        if (hit.collider.name.Contains("Chunk"))
        //        {
        //            transform.position = hit.point;
        //            print($"Settings {name} position to {transform.position}");
        //            break;
        //        }
        //    }
        //}
        float meshWorldSize = meshSettings.meshWorldSize;
        float xPos = transform.position.x;
        float zPos = transform.position.z;
        Vector2 thisLocation = new Vector2(xPos, zPos);
        float[,] hm = Noise.GenerateNoiseMap(1, 1, heightMapSettings.noiseSettings, thisLocation);
        float height = hm[0, 0];
        AnimationCurve heightCurve_threadsafe = new AnimationCurve(heightMapSettings.heightCurve.keys);
        height *= heightCurve_threadsafe.Evaluate(height) * heightMapSettings.heightMultiplier;
        transform.position = new Vector3(xPos, height, zPos);
    }

    // Update is called once per frame

}

