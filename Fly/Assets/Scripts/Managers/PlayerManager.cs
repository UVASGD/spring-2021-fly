using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Player activePlayer;

    public Player SpawnPlayerAtPosition(Vector3 position, Model.Type type = Model.Type.Classic)
    {
        activePlayer = Instantiate(playerPrefab, position, Quaternion.identity).GetComponent<Player>();
        activePlayer?.modelController?.SetActiveModel(type);
        return activePlayer;
    }

    public Player SpawnPlayerAtObject(GameObject obj, Model.Type type = Model.Type.Classic)
    {
        activePlayer = Instantiate(playerPrefab, obj.transform.position, obj.transform.rotation).GetComponent<Player>();
        activePlayer.modelController.SetActiveModel(type);
        return activePlayer;
    }
}
