using System;
using MergeIt.Core.Messages;
using MergeIt.Game;
using MergeIt.Game.Services;
using MergeIt.SimpleDI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour
{
    private const string LAST_RECEIVED_REWARD = "LastReceivedReward";
    private const string UPGRADE_BUTTON_AVAILABLE_TEXT = "Upgrade available!";
    private const string UPGRADE_BUTTON_NOT_AVAILABLE_TEXT = "Level up for your next upgrade!";
    // Configs
    [SerializeField] private UpgradeList _upgradeRewardsList;

    [SerializeField] private StageBackgroundLogic _stageBackgroundLogic;
    
    // Upgrade available menu button
    [SerializeField] private TextMeshProUGUI _upgradeAvailableButtonText;
    [SerializeField] private Button _upgradeAvailableButton;
    
    // Upgrade panel
    [SerializeField] private RectTransform _upgradePanel;
    [SerializeField] private TextMeshProUGUI _currentLevelText;
    [SerializeField] private TextMeshProUGUI _currentRewardNameText;
    [SerializeField] private Image _currentRewardImage;    
    [SerializeField] private TextMeshProUGUI _nextRewardNameText;
    [SerializeField] private Image _nextRewardImage;
    [SerializeField] private Button _collectRewardButton;

    private IMessageBus  _messageBus;
    private UserServiceModel _userServiceModel;
    
    private int _playerLevel;

    private void Awake()
    {
        _upgradePanel.gameObject.SetActive(false);
    }

    public void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
        _messageBus.AddListener<MenuStartedMessage>(OnMenuStartedMessage);        
    }

    private void OnMenuStartedMessage(MenuStartedMessage obj)
    {
        _userServiceModel = DiContainer.Get<UserServiceModel>();
        _playerLevel = _userServiceModel.Level.Value;
        
        _upgradeAvailableButton.onClick.AddListener(ShowPanel);
        _collectRewardButton.onClick.AddListener(CollectReward);

        CheckUpgradeAvailable();
    }

    private void CheckUpgradeAvailable()
    {
        if (GetCurrentUpgradeReward())
        {
            ActivateUpgradeButton();
        }
        else
        {
            DeactivateUpgradeButton();
        }
    }
    
    private void ActivateUpgradeButton()
    {
        _upgradeAvailableButtonText.text = UPGRADE_BUTTON_AVAILABLE_TEXT;
        _upgradeAvailableButton.interactable = true;
    } 
    
    private void DeactivateUpgradeButton()
    {
        _upgradeAvailableButtonText.text = UPGRADE_BUTTON_NOT_AVAILABLE_TEXT;
        _upgradeAvailableButton.interactable = false;
    }
    
    private void ShowPanel()
    {
        UpgradeRewardDefinition reward = GetCurrentUpgradeReward();
        
        _currentLevelText.text = _playerLevel.ToString();

        _currentRewardNameText.text = reward.RewardName;
        _currentRewardImage.sprite = reward.RewardSprite;
        var nextReward = GetNextUpgradeReward();
        
        if (nextReward)
        {
            _nextRewardNameText.text = nextReward.RewardName;
            _nextRewardImage.sprite = nextReward.RewardSprite;
        }
        else
        {
            Debug.Log("we need to add more rewards for the next levels!");
        }
        
        _upgradePanel.gameObject.SetActive(true);
    }
    
    private void CollectReward()
    {
        _upgradePanel.gameObject.SetActive(false);
        CheckUpgradeAvailable();
        
        UpgradeRewardDefinition reward = GetCurrentUpgradeReward();
        
        // save player prefs
        PlayerPrefs.SetInt(LAST_RECEIVED_REWARD, reward.LevelRequirement);
        
        // animate reward on the menu
        _stageBackgroundLogic.AddUpgradeOnStage(reward);
        
        
        // TODO LATER: Check if it is end of stage and move to the other stage
    }

    private UpgradeRewardDefinition GetCurrentUpgradeReward()
    {
        foreach (var rewardDefinition in _upgradeRewardsList.UpgradeRewardDefinitions)
        {
            if (_playerLevel >= rewardDefinition.LevelRequirement)
            {
                if (rewardDefinition.LevelRequirement > PlayerPrefs.GetInt(LAST_RECEIVED_REWARD))
                {
                    return rewardDefinition;
                }
            }
        }
        
        return null;
    }
    
    private UpgradeRewardDefinition GetNextUpgradeReward()
    {
        foreach (var rewardDefinition in _upgradeRewardsList.UpgradeRewardDefinitions)
        {
            if (_playerLevel + 1 >= rewardDefinition.LevelRequirement)
            {
                if (rewardDefinition.LevelRequirement > PlayerPrefs.GetInt(LAST_RECEIVED_REWARD, 1) + 1)
                {
                    return rewardDefinition;
                }
            }
        }
        
        return null;
    }
}
