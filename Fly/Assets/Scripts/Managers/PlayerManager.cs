using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaneModelStats
{

}

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Player activePlayer;

    public Player SpawnPlayerAtPosition(Vector3 position)
    {
        activePlayer = Instantiate(playerPrefab, position, Quaternion.identity).GetComponent<Player>();
        return activePlayer;
    }

    public Player SpawnPlayerAtObject(GameObject obj)
    {
        activePlayer = Instantiate(playerPrefab, obj.transform.position, obj.transform.rotation).GetComponent<Player>();
        return activePlayer;
    }
}
