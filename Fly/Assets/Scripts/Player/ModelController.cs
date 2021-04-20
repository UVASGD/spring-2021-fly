using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour, ISavable
{
    public List<Model> models;
    public Model activeModel;

    public void Load()
    {
        if (PlayerPrefs.HasKey("planeModel"))
        {
            int index = PlayerPrefs.GetInt("planeModel");
            activeModel = models[index];
            SyncActiveModel();
        }
        else
        {
            throw new System.NullReferenceException();
        }
    }

    public void Save()
    {
        int index = models.IndexOf(activeModel);
        PlayerPrefs.SetInt("planeModel", index);
    }

    public void SetActiveModel(Model.Type type)
    {
        foreach (var model in models)
        {
            if (model.type == type)
            {
                model.gameObject.SetActive(true);
                activeModel = model;
            }
            else
            {
                model.gameObject.SetActive(false);
            }
        }
        Player.instance.playerController.model = activeModel.gameObject.transform;
    }

    public void SyncActiveModel()
    {
        SetActiveModel(activeModel.type);
    }

    
}

[System.Serializable]
public class Model
{
    public Type type;
    public GameObject gameObject;

    public enum Type
    {
        Classic,
        BigFlat,
        Needlenose,
        Stingray,
        Cobra,
    }
    

}
    /*static class UpgradablePropertiesMethods {
        //getWeight()
        //getVelocityDecrease()
        //getFuelUse()
        

        public static float getWeight(this Type type1) {
            switch (type) {
                case Type.Classic:
                    return -0.1f;
                case Type.BigFlat:
                    return -0.2f;
                case Type.Needlenose:
                    return -0.3f;
                case Type.Stingray:
                    return -0.4f;
                case Type.Cobra:
                    return -0.5f;
                default:
                    return 100f;
            }
        }
    }
    */
    