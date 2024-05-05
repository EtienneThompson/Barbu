namespace Barbu
{
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;
    using System.Threading.Tasks;
    using UnityEngine;

    public class ShowAdvertisementStep : IStep<RoundArguments>
    {
        private IWorkflow workflow;
        private StateMachine stateMachine;
        private ITelemetryService telemetryService;

        public void Initialize(IWorkflow workflow, StateMachine stateMachine, ITelemetryService telemetryService)
        {
            this.workflow = workflow;
            this.stateMachine = stateMachine;
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
