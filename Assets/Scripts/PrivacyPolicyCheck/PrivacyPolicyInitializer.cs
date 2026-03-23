using System;
using UnityEngine;
using UnityEngine.UI;

public class PrivacyPolicyInitializer : MonoBehaviour
{
    private const  string PRIVACY_POLICY_ACCEPTED_KEY = "PrivacyPolicyHasBeenCleared";
    [SerializeField] private GameObject PrivacyPolicy;
    [SerializeField] private Button PrivacyPolicyLink_button;
    [SerializeField] private Button PrivacyPolicyAccept_button;

    private bool PrivacyPolicyHasBeenCleared
    {
        get => PlayerPrefs.GetInt(PRIVACY_POLICY_ACCEPTED_KEY, 0) > 0;
        set => PlayerPrefs.SetInt(PRIVACY_POLICY_ACCEPTED_KEY, value ? 1 : 0);
    }

    private void Awake()
    {
        if (!PrivacyPolicyHasBeenCleared)
        {
            PrivacyPolicyLink_button.onClick.AddListener(OnPrivacyPolicyLinkClicked);
            PrivacyPolicyAccept_button.onClick.AddListener(OnPrivacyPolicyAccepted);
            PrivacyPolicy.SetActive(true);
        }
        else
        {
            PrivacyPolicyHasBeenCleared = true;
        }
    }
    
    private void OnPrivacyPolicyAccepted()
    {
        PrivacyPolicyHasBeenCleared = true;
        PrivacyPolicy.SetActive(false);
    }

    private void OnPrivacyPolicyLinkClicked()
    {
        Application.OpenURL("https://www.srbvpublishing.com/privacy-policy");
    }
}
