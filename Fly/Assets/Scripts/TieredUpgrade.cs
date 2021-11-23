using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TieredUpgrade
{
    public enum Type
    {
        Chonk,
        Dynamics,
        Grit,
        Spunk,
        RocketScience,
        Design
    }

    public Type type;
    public List<UpgradeTier> tiers;
}

[System.Serializable]
public class UpgradeTier
{
    public enum Type
    {
        Multiply,
        Add,
    }

    public Type upgradeType;
    public float value = 1.0f;
}
