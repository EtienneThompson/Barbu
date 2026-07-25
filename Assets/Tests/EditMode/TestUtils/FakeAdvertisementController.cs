namespace Barbu.Tests.EditMode.TestUtils
{
    using Barbu.Core;

    /// <summary>IAdvertisementController double that records calls instead of talking to the real Unity Ads SDK.</summary>
    public class FakeAdvertisementController : IAdvertisementController
    {
        public int RequestToShowInterstitialCallCount { get; private set; }

        public int ShowInterstitialAdCallCount { get; private set; }

        public void RequestToShowInterstitial()
        {
            this.RequestToShowInterstitialCallCount++;
        }

        public void ShowInterstitialAd()
        {
            this.ShowInterstitialAdCallCount++;
        }
    }
}
