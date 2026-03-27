using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdMobRewardedAdService
{
    private readonly string _adUnitId;
    private RewardedAd _rewardedAd;

    private int _retryAttempt;

    public event Action<string> OnAdFailedToLoad;
    public event Action<string> OnAdFailedToShow;
    public event Action OnAdClosed;
    public event Action OnAdNotReady;

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
        
                string errorMsg = error?.ToString() ?? "Unknown load error";
				OnAdFailedToLoad?.Invoke(errorMsg); // 🔥 ADD THIS
                Debug.LogError($"Ad failed to load: {error?.GetCode()} - {error?.GetMessage()}");
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
            OnAdNotReady?.Invoke(); // 🔥 ADD THIS
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
            OnAdClosed?.Invoke(); // 🔥 ADD THIS
            Load();
        };

        ad.OnAdFullScreenContentFailed += error =>
        {
            Debug.LogError($"Ad failed to show: {error}");
            string errorMsg = error.ToString();
            OnAdFailedToShow?.Invoke(errorMsg); // 🔥 ADD THIS
            Load();
        };
    }
}