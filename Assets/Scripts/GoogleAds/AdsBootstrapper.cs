using GoogleMobileAds.Api;
using UnityEngine;

public class AdsBootstrapper : MonoBehaviour
{
    public static AdMobRewardedAdService RewardedService { get; private set; }

    [SerializeField] private string _adUnitId = "ca-app-pub-7237372029635198/8986768802";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob Initialized");

            RewardedService = new AdMobRewardedAdService(_adUnitId);
            RewardedService.Load();
        });
    }
}