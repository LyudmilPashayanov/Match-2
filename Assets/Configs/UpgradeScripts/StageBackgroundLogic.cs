using System;
using System.Collections.Generic;
using DG.Tweening;
using MergeIt.Core.Messages;
using MergeIt.Game;
using MergeIt.Game.Services;
using MergeIt.SimpleDI;
using UnityEngine;
using UnityEngine.UI;

public class StageBackgroundLogic : MonoBehaviour
{
    private const string LAST_RECEIVED_REWARD = "LastReceivedReward";
    [SerializeField] public Stage stageType;
    [SerializeField] public List<SpawnedReward> spawnedRewards;
    [SerializeField] public Image _hideCanvasImage;

    private IMessageBus  _messageBus;
    private UserServiceModel _userServiceModel;

    private void Awake()
    {
        _hideCanvasImage.gameObject.SetActive(true);
    }

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

        _hideCanvasImage.DOFade(0, 1f).OnComplete(
            () => _hideCanvasImage.gameObject.SetActive(false)
            );
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
