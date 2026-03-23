using UnityEngine;

public static class AdsEntitlement
{
    private const string NoAdsKey = "NoAdsPurchased";

    public static bool HasNoAds()
    {
        return PlayerPrefs.GetInt(NoAdsKey, 0) == 1;
    }

    public static void SetNoAds(bool value)
    {
        PlayerPrefs.SetInt(NoAdsKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}