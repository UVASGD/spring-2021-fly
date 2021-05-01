using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plane Upgrades", menuName = "ScriptableObjects/Plane Upgrade List")]
public class TieredUpgradeList : ScriptableObject
{
    public List<TieredUpgrade> upgrades;
}

[System.Serializable]
public class TieredUpgrade
{
    public enum Type
    {
        Chonk,
        Dynamics,
        Grit,
        Spunk,
        Science,
        Design
    }

    public Type type;
    public List<UpgradeTier> tiers;
    public int activeTierIndex;
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
