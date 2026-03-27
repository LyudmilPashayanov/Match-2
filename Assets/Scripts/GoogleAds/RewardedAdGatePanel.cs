using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardedAdGatePanel : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private RectTransform _toastPanel;
    [SerializeField] private RectTransform _noInternetPanel;

    [SerializeField] private Button _watchAdButton;
    [SerializeField] private Button _buyNoAdsButton;
    [SerializeField] private Button _closeToastButton;
    [SerializeField] private Button _closeInternetPanelButton;
    [SerializeField] private TextMeshProUGUI _toastMessage;
    private System.Action _onAdCompleted;
    
    private bool HasInternet() => Application.internetReachability != NetworkReachability.NotReachable;
    
    
    public bool IsOpen => _root.activeSelf;

    private void Awake()
    {
        _toastPanel.gameObject.SetActive(false);
        OnCloseNoInternetPanel();
        _watchAdButton.onClick.AddListener(OnWatchAdClicked);
        _buyNoAdsButton.onClick.AddListener(OnBuyNoAdsClicked);
        _closeToastButton.onClick.AddListener(OnCloseToastClicked);
        _closeInternetPanelButton.onClick.AddListener(OnCloseNoInternetPanel);
        _root.SetActive(false);
    }

    private void Start()
    {
        SubscribeEvents();
    }

    private void OnCloseToastClicked()
    {
        _toastPanel.gameObject.SetActive(false);
        OnAdFinished();
    }

    private void ShowToast(string message)
    {
        OnCloseNoInternetPanel();
        _toastPanel.gameObject.SetActive(true);
        _toastMessage.text =  message;
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
    
    private void ShowNoInternetPanel()
    {
        _noInternetPanel.gameObject.SetActive(true);
    }

    private void OnCloseNoInternetPanel()
    {
        _noInternetPanel.gameObject.SetActive(false);
    }
    
    private void OnWatchAdClicked()
    {
        if (!HasInternet())
        {
            ShowNoInternetPanel();
            return;
        }

        _watchAdButton.interactable = false;

        AdsBootstrapper.RewardedService.Show(OnAdFinished);
    }

    private void OnAdFinished()
    {
        Debug.Log("Ad watched → closing panel");

        _toastPanel.gameObject.SetActive(false);
        OnCloseNoInternetPanel();
        _watchAdButton.interactable = true;

        _onAdCompleted?.Invoke();
        Close();
    }

    private void HandleAdFailedToLoad(string error)
    {
        ShowToast("Ad is not available right now. Please try again later.");
    }

    private void HandleAdFailedToShow(string error)
    {
        ShowToast("Something went wrong while showing the ad.");
        _watchAdButton.interactable = true;
    }

    private void HandleAdNotReady()
    {
        ShowToast("Ad is still loading. Please wait a moment.");
    }
    
    private void HandleAdClosed()
    {
        _watchAdButton.interactable = true;
        OnCloseNoInternetPanel();
        _toastPanel.gameObject.SetActive(false);
    }
    
    private void SubscribeEvents()
    {
        var service = AdsBootstrapper.RewardedService;

        service.OnAdFailedToLoad += HandleAdFailedToLoad;
        service.OnAdFailedToShow += HandleAdFailedToShow;
        service.OnAdNotReady += HandleAdNotReady;
        service.OnAdClosed += HandleAdClosed;
    }
    
    private void OnDestroy()
    {
        if (AdsBootstrapper.RewardedService == null)
            return;

        var service = AdsBootstrapper.RewardedService;

        service.OnAdFailedToLoad -= HandleAdFailedToLoad;
        service.OnAdFailedToShow -= HandleAdFailedToShow;
        service.OnAdNotReady -= HandleAdNotReady;
        service.OnAdClosed -= HandleAdClosed;
    }
}