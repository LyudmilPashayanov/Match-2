using UnityEngine;

public class RewardedAdScheduler : MonoBehaviour
{
    private const string PendingAdKey = "PendingRewardedAd";

    [SerializeField] private float _intervalSeconds = 7f;
    [SerializeField] private RewardedAdGatePanel _panel;

    private float _timeSinceLastTrigger;

    private void Start()
    {
        if (AdsEntitlement.HasNoAds())
            return;
        
        // If player previously didn't watch the ad → force panel again
        if (PlayerPrefs.GetInt(PendingAdKey, 0) == 1)
        {
            ShowGate();
        }
    }

    private void Update()
    {
        if (AdsEntitlement.HasNoAds())
            return;
        
        if (!_panel)
        {
            _panel = FindAnyObjectByType<RewardedAdGatePanel>();
        }
        if (_panel.IsOpen)
            return;

        if (AdsBootstrapper.RewardedService == null)
            return;

        _timeSinceLastTrigger += Time.deltaTime;

        if (_timeSinceLastTrigger >= _intervalSeconds)
        {
            TriggerGate();
        }
    }

    private void TriggerGate()
    {
        if (!CanInterruptGameplay())
            return;

        PlayerPrefs.SetInt(PendingAdKey, 1);
        PlayerPrefs.Save();

        ShowGate();
    }

    private void ShowGate()
    {
        _panel.Open(OnAdCompleted);
    }

    private void OnAdCompleted()
    {
        Debug.Log("Ad completed → clearing gate");

        PlayerPrefs.SetInt(PendingAdKey, 0);
        PlayerPrefs.Save();

        _timeSinceLastTrigger = 0f;
    }

    private bool CanInterruptGameplay()
    {
        return true;
    }
}