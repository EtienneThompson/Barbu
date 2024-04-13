namespace Barbu.Core
{
    using UnityEngine;
    using UnityEngine.Advertisements;

    public class AdvertisementController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] private string adId;
        [SerializeField] private bool initialized;
        [SerializeField] private bool adLoaded;

        public void Awake()
        {
            this.InitializeAds();
        }

        public void InitializeAds()
        {
            Debug.Log("Initializing ads");
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                this.adId = Constants.AdGameIds.AppleGameId;
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                this.adId = Constants.AdGameIds.AndroidGameId;
            }
            else
            {
                this.adId = Constants.AdGameIds.AndroidGameId;
            }

            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(this.adId, false, this);
            }
        }

        public void RequestToShowInterstitial()
        {
            // Don't double load ads.
            if (!this.initialized)
            {
                Debug.Log("Ads are not yet initialized");
                return;
            }

            if (this.adLoaded)
            {
                Debug.Log("Already requested an interstitial");
                return;
            }

            Debug.Log("Requesting to show interstitial ad to user");
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Advertisement.Load(Constants.AdGameIds.AppleInterstitialId, this);
            }
            else
            {
                Advertisement.Load(Constants.AdGameIds.AndroidInterstitialId, this);
            }
        }

        public void ShowInterstitialAd()
        {
            // Don't show if nothing was loaded.
            if (!this.initialized)
            {
                Debug.Log("Ads are not yet initialized");
                return;
            }

            if (!this.adLoaded)
            {
                Debug.Log("No interstitial ad loaded");
                return;
            }

            string placementId;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                placementId = Constants.AdGameIds.AppleInterstitialId;
            }
            else
            {
                placementId = Constants.AdGameIds.AndroidInterstitialId;
            }

            Debug.Log("Showing ad");
            Advertisement.Show(placementId, null, this);
            this.adLoaded = false;
        }

        public void OnInitializationComplete()
        {
            this.initialized = true;
            // Because of the asynchronous nature of ads initialization, once we are
            // initialized, request one because we might have requested an ad via the
            // round manager before we actually were initialized.
            this.RequestToShowInterstitial();
            Debug.Log("Unity Ads initialization complete.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            this.initialized = false;
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            this.adLoaded = true;
            Debug.Log($"Unity Ads Ad Loaded complete - {placementId}");
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            this.adLoaded = false;
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
}
