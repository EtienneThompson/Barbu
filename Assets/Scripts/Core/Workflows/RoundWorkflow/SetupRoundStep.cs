namespace Barbu.Core.Workflows.RoundWorkflow
{
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using System.Threading.Tasks;
    using UnityEngine;

    public class SetupRoundStep : IStep<RoundArguments>
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
            this.telemetryService.LogInfo("[RoundWorkflow] [SetupRound] Executing setup round step...");

            var gameBoard = GameObject.Find(Constants.GameObjects.GameBoard);
            var gameBoardController = gameBoard.GetComponent<GameBoard>();
            var hands = gameBoardController.SetupRound();
            args.Data.Hands = hands;

            this.workflow.SetNextStep(nameof(PreRoundStep));
            return Task.CompletedTask;
        }
    }
}
