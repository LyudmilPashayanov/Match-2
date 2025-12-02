using System.Collections.Generic;
using MergeIt.Core.Messages;
using MergeIt.Game;
using MergeIt.Game.Services;
using MergeIt.SimpleDI;
using UnityEngine;

public class StageBackgroundLogic : MonoBehaviour
{
    private const string LAST_RECEIVED_REWARD = "LastReceivedReward";
    [SerializeField] public Stage stageType;
    [SerializeField] public List<SpawnedReward> spawnedRewards;

    private IMessageBus  _messageBus;
    private UserServiceModel _userServiceModel;

    public void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
        _messageBus.AddListener<MenuStartedMessage>(OnMenuStartedMessage);        
    }

    private void OnMenuStartedMessage(MenuStartedMessage _)
    {
        _userServiceModel = DiContainer.Get<UserServiceModel>();
        int lastRewardLevel = PlayerPrefs.GetInt(LAST_RECEIVED_REWARD);
        
        foreach (var reward in spawnedRewards)
        {
            if (reward.RewardDefinition.LevelRequirement <= lastRewardLevel)
            {
                reward.Show();
            }
            else
            {
                reward.Hide();
            }
        }
    }

    public void AddUpgradeOnStage(UpgradeRewardDefinition reward)
    {
        foreach (var spawnedReward in spawnedRewards)
        {
            if (spawnedReward.RewardDefinition == reward)
            {
                spawnedReward.AnimateIn();
                return;
            }
        }
    }
}
