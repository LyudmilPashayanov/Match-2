using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeList", menuName = "Game/Upgrade List")]
public class UpgradeList : ScriptableObject
{
    public List<UpgradeRewardDefinition> UpgradeRewardDefinitions = new List<UpgradeRewardDefinition>();
} 