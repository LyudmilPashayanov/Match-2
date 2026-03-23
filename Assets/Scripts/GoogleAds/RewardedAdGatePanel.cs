using UnityEngine;
using UnityEngine.UI;

public class RewardedAdGatePanel : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Button _watchAdButton;
    [SerializeField] private Button _buyNoAdsButton;
   // [SerializeField] private GameObject _loadingIndicator;

    private System.Action _onAdCompleted;

    public bool IsOpen => _root.activeSelf;

    private void Awake()
    {
        _watchAdButton.onClick.AddListener(OnWatchAdClicked);
        _buyNoAdsButton.onClick.AddListener(OnBuyNoAdsClicked);
        _root.SetActive(false);
    }

    private void OnBuyNoAdsClicked()
    {
        if (UnityIAPManager.Instance == null)
        {
            Debug.LogError("IAP Manager not found.");
            return;
        }

        if (!UnityIAPManager.Instance.CanPurchaseNoAds())
        {
            Debug.Log("Cannot purchase No Ads (already owned or not initialized).");
            return;
        }

        Debug.Log("Buying No Ads...");

        UnityIAPManager.Instance.BuyNoAds();    
    }

    public void Open(System.Action onAdCompleted)
    {
        if (AdsEntitlement.HasNoAds())
            return;
        
        _onAdCompleted = onAdCompleted;
        _root.SetActive(true);
    }

    private void Close()
    {
        _root.SetActive(false);
    }

    private void OnWatchAdClicked()
    {
        if (!AdsBootstrapper.RewardedService.IsReady())
        {
            Debug.Log("Ad not ready → show feedback");
            return;
        }

        _watchAdButton.interactable = false;
        //_loadingIndicator.SetActive(true);

        AdsBootstrapper.RewardedService.Show(OnAdFinished);
    }

    private void OnAdFinished()
    {
        Debug.Log("Ad watched → closing panel");

        //_loadingIndicator.SetActive(false);
        _watchAdButton.interactable = true;

        _onAdCompleted?.Invoke();
        Close();
    }
}