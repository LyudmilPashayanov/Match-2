using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdMobRewardedAdService
{
    private readonly string _adUnitId;
    private RewardedAd _rewardedAd;

    private int _retryAttempt;

    public AdMobRewardedAdService(string adUnitId)
    {
        _adUnitId = adUnitId;
    }

    public void Load()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        var request = new AdRequest();

        RewardedAd.Load(_adUnitId, request, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                _retryAttempt++;
                float delay = Mathf.Pow(2, _retryAttempt);

                Debug.LogError($"Ad failed to load. Retrying in {delay}s");
                TimerRunner.Instance.RunAfter(delay, Load); // we'll define this
                return;
            }

            _retryAttempt = 0;
            _rewardedAd = ad;

            RegisterEvents(ad);

            Debug.Log("Rewarded ad loaded.");
        });
    }

    public bool IsReady()
    {
        return _rewardedAd != null && _rewardedAd.CanShowAd();
    }

    public void Show(Action onRewarded)
    {
        if (!IsReady())
        {
            Debug.LogWarning("Ad not ready.");
            return;
        }

        _rewardedAd.Show(reward =>
        {
            Debug.Log($"Reward: {reward.Amount} {reward.Type}");
            onRewarded?.Invoke();
        });
    }

    private void RegisterEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Ad closed → reloading");
            Load();
        };

        ad.OnAdFullScreenContentFailed += error =>
        {
            Debug.LogError($"Ad failed to show: {error}");
            Load();
        };
    }
}