using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainObject
{

    public GameObject prefab;
    public int maxPerChunk;
    public int minPerChunk;

    public float defaultYPosition = 0;

    public bool SnapToGround = false;
    public bool restrictYSpawn = false;
    public float minYSpawn = 0.0f;
    public float maxYSpawn = 0.0f;

    public bool spawnInMiddle = false;
}
