namespace Barbu.Core.Workflows.RoundWorkflow
{
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using System.Threading.Tasks;
    using UnityEngine;

    public class ShowAdvertisementStep : IStep<RoundArguments>
    {
        private IWorkflow workflow;
        private ITelemetryService telemetryService;

        public void Initialize(
            IWorkflow workflow,
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.workflow = workflow;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [ShowAdvertisement] Executing show advertisement step...");

            var gameBoard = GameObject.Find(Constants.GameObjects.GameBoard);
            var advertisementController = gameBoard.GetComponent<AdvertisementController>();
            advertisementController.ShowInterstitialAd();

            this.workflow.SetNextStep(null);
            return Task.CompletedTask;
        }
    }
}
