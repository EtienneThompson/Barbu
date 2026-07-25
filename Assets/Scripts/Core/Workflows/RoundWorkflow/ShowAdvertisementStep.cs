namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using Zenject;

    public class ShowAdvertisementStep : IStep<RoundArguments>
    {
        public class Factory : PlaceholderFactory<ShowAdvertisementStep>
        {
        }

        private readonly IRoundOverlayController roundOverlayController;
        private readonly IAdvertisementController advertisementController;
        private readonly ITelemetryService telemetryService;

        public ShowAdvertisementStep(
            IRoundOverlayController roundOverlayController,
            IAdvertisementController advertisementController,
            ITelemetryService telemetryService)
        {
            this.roundOverlayController = roundOverlayController;
            this.advertisementController = advertisementController;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [ShowAdvertisement] Executing show advertisement step...");

            this.roundOverlayController.SetActive(false);
            this.roundOverlayController.HideText();

            this.advertisementController.ShowInterstitialAd();

            args.Workflow.SetNextStep(null);
            return Task.CompletedTask;
        }
    }
}
