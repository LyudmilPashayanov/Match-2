using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpawnedReward : MonoBehaviour
{
    private const float SCALE_UP_ANIMATION_DURATION = 3f;
    
    [SerializeField] private Image _rewardImage;
    [SerializeField] public UpgradeRewardDefinition RewardDefinition;
    [SerializeField] private Vector3 ShowSize = Vector3.one;

    private void Start()
    {
        _rewardImage.sprite = RewardDefinition.RewardSprite;
    }

    public void AnimateIn()
    {
        transform.DOScale(ShowSize, SCALE_UP_ANIMATION_DURATION);
    }
    
    public void Show()
    {
        transform.localScale = ShowSize;
    }

    public void Hide()
    {
        transform.localScale = Vector3.zero;
    }
}
