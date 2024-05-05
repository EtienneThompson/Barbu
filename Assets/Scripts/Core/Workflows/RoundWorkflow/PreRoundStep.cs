namespace Barbu
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models;
    using Barbu.Models.Workflows;
    using Barbu.UI.Controllers;
    using UnityEngine;

    public class PreRoundStep : IStep<RoundArguments>
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
            this.telemetryService.LogInfo("[RoundWorkflow] Executing PreRound step...");
            GameObject roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
            var controller = roundOverlay.GetComponent<RoundOverlayController>();
            controller.DisplayRound(args.Data.GetCurrentRound().Name);

            GameObject inGamePointsOverlay = GameObject.Find(Constants.GameObjects.InGamePoints);
            var inGamePointsController = inGamePointsOverlay.GetComponent<InGamePointsController>();
            inGamePointsController.ResetRoundName();

            this.workflow.SetNextStep(nameof(StartRoundStep));
            this.workflow.WaitForEvent(EventNames.RoundAnimationFinished);

            return Task.CompletedTask;
        }
    }
}
