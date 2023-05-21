using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertisementController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string adId;

    public void Awake()
    {
        this.InitializeAds();
    }

    public void InitializeAds()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            this.adId = Constants.AdGameIds.AppleGameId;
        }
        else
        {
            this.adId = Constants.AdGameIds.AndroidGameId;
        }

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(this.adId, true, this);
        }
    }

    public void RequestToShowInterstitial()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Advertisement.Load(Constants.AdGameIds.AppleInterstitialId, this);
        }
        else
        {
            Advertisement.Load(Constants.AdGameIds.AndroidInterstitialId, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Unity Ads Ad Loaded complete - {placementId}");
        Advertisement.Show(placementId);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Unity Ads Ad failed to load: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Unity Ads ad was clicked on");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"Unity Ads ad was completed");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Unity Ads as failed to show: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Unity Ads ad has started showing");
    }
}
