using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Player activePlayer;

    public void SpawnPlayerAtPosition(Vector3 position)
    {
        activePlayer = Instantiate(playerPrefab, position, Quaternion.identity, transform).GetComponent<Player>();
    }

    public void SpawnPlayerAtObject(GameObject obj)
    {
        activePlayer = Instantiate(playerPrefab, obj.transform.position, obj.transform.rotation, transform).GetComponent<Player>();
    }
}
