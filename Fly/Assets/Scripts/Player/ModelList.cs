using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Model List", menuName = "ScriptableObjects/Model List")]
public class ModelList : ScriptableObject
{
    public List<Model> models;
    public Dictionary<Model.Type, Model> modelMap;

    // Set up dictionary for intuitive reference by type
    public void Init()
    {
        modelMap = new Dictionary<Model.Type, Model>();

        foreach (var item in models)
        {
            modelMap.Add(item.type, item);
        }
    }
}

[System.Serializable]
public class Model
{
    public Type type;
    public Stats stats;
    public GameObject prefab;


    #region DEFINITIONS
    public enum Type
    {
        Classic,
        BigFlat,
        Needlenose,
        Stingray,
        Cobra,
    }

    [System.Serializable]
    public class Stats
    {
        [Range(0f, 2f)]
        public float weightMultiplier;
        [Range(0f, 2f)]
        public float initialVelocityMultiplier;
        [Range(0f, 2f)]
        public float dragMultiplier;
        [Range(0f, 2f)]
        public float fuelMultiplier;
        [Range(0f, 2f)]
        public float bounceHeightMultiplier;
    }
    #endregion
}