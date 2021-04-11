using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Player activePlayer;

    public Player SpawnPlayerAtPosition(Vector3 position)
    {
        activePlayer = Instantiate(playerPrefab, position, Quaternion.identity).GetComponent<Player>();
        activePlayer.modelController.SetActiveModel(Model.Type.FlatSpace);
        return activePlayer;
    }

    public Player SpawnPlayerAtObject(GameObject obj)
    {
        activePlayer = Instantiate(playerPrefab, obj.transform.position, obj.transform.rotation).GetComponent<Player>();
        activePlayer.modelController.SetActiveModel(Model.Type.FlatSpace);
        return activePlayer;
    }
}
