using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour, ISavable
{
    public ModelList modelList; // ScriptableObject found somewhere in Assets folder
    [HideInInspector] public Model activeModel;

    public void Init()
    {
        Load();
        modelList.Init();
        int index = UpgradeManager.instance.tieredUpgradeList.upgrades[5].activeTierIndex;
        print("INDEX: " + index);
        activeModel = modelList.models[index];
    }

    public void SetActiveModel(Model model)
    {
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        GameObject obj = Instantiate(model.prefab, transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        Player.instance.playerController.model = obj.transform;
    }

    public void SyncActiveModel()
    {
        SetActiveModel(activeModel);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("planeModel"))
        {
            int index = PlayerPrefs.GetInt("planeModel");
            activeModel = modelList.models[index];
            SyncActiveModel();
        }
        else
        {
            activeModel = modelList.models[0];
            SyncActiveModel();
        }
    }

    public void Save()
    {
        int index = modelList.models.IndexOf(activeModel);
        PlayerPrefs.SetInt("planeModel", index);
    }
}