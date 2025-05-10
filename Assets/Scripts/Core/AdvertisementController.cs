namespace Barbu.Core
{
    using Barbu.Core.Telemetry;
    using UnityEngine;
    using UnityEngine.Advertisements;
    using Zenject;

    public class AdvertisementController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] private string adId;
        [SerializeField] private bool initialized;
        [SerializeField] private bool adLoaded;
        private ITelemetryService telemetryService;

        [Inject]
        public void Init(ITelemetryService telemetryService)
        {
            this.telemetryService = telemetryService;
        }

        public void Awake()
        {
            this.InitializeAds();
        }

        public void InitializeAds()
        {
            this.telemetryService.LogInfo("Initializing ads");
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
                this.telemetryService.LogInfo("Ads are not yet initialized");
                return;
            }

            if (this.adLoaded)
            {
                this.telemetryService.LogInfo("Already requested an interstitial");
                return;
            }

            this.telemetryService.LogInfo("Requesting to show interstitial ad to user");
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
                this.telemetryService.LogInfo("Ads are not yet initialized");
                return;
            }

            if (!this.adLoaded)
            {
                this.telemetryService.LogInfo("No interstitial ad loaded");
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

            this.telemetryService.LogInfo("Showing ad");
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
            this.telemetryService.LogInfo("Unity Ads initialization complete.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            this.initialized = false;
            this.telemetryService.LogError($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            this.adLoaded = true;
            this.telemetryService.LogInfo($"Unity Ads Ad Loaded complete - {placementId}");
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            this.adLoaded = false;
            this.telemetryService.LogError($"Unity Ads Ad failed to load: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            this.telemetryService.LogInfo($"Unity Ads ad was clicked on");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            this.telemetryService.LogInfo($"Unity Ads ad was completed");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            this.telemetryService.LogError($"Unity Ads as failed to show: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            this.telemetryService.LogInfo($"Unity Ads ad has started showing");
        }
    }
}
