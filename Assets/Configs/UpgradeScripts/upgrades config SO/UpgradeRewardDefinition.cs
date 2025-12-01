using UnityEngine;

public enum Stage
{
    Stage1,
    Stage2,
    Stage3
}
[CreateAssetMenu(fileName = "UpgradeRewardDefinition", menuName = "Game/UpgradeRewardDefinition")]
public class UpgradeRewardDefinition : ScriptableObject
{
    public int LevelRequirement;
    public Stage StageType;
    public string RewardName;
    public Sprite RewardSprite;}

