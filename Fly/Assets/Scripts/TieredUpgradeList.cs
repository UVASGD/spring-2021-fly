using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plane Upgrades", menuName = "ScriptableObjects/Plane Upgrade List")]
public class TieredUpgradeList : ScriptableObject
{
    public List<TieredUpgrade> upgrades;
}