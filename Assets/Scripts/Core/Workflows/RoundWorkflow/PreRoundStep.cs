namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using UnityEngine;

    public class PreRoundStep : IStep<RoundArguments>
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
            this.telemetryService.LogInfo("[RoundWorkflow] Executing PreRound step...");
            GameObject roundOverlay = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.RoundOverlay, findInactive: true);
            roundOverlay.SetActive(true);
            var controller = roundOverlay.GetComponent<RoundOverlayController>();
            controller.DisplayRound(args.Data.GetCurrentRound().Name, args.Data.GameType);

            GameObject inGamePointsOverlay = GameObject.Find(Constants.GameObjects.InGamePoints);
            var inGamePointsController = inGamePointsOverlay.GetComponent<InGamePointsController>();
            inGamePointsController.ResetRoundName();

            this.workflow.SetNextStep(nameof(StartRoundStep));
            this.workflow.WaitForEvent(EventNames.RoundAnimationFinished);

            return Task.CompletedTask;
        }
    }
}
