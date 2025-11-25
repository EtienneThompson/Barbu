namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using UnityEngine;

    public class CleanupRoundStep : IStep<RoundArguments>
    {
        private IWorkflow workflow;
        private IStateMachine stateMachine;
        private ITelemetryService telemetryService;

        public void Initialize(
            IWorkflow workflow,
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.workflow = workflow;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [CleanupRound] Executing CleanupRound step...");

            var gameBoard = GameObject.Find(Constants.GameObjects.GameBoard);
            var gameBoardController = gameBoard.GetComponent<GameBoard>();
            gameBoardController.CleanupRound();

            var inGamePointsOverlay = GameObject.Find(Constants.GameObjects.InGamePoints);
            var inGamePointsController = inGamePointsOverlay.GetComponent<InGamePointsController>();
            inGamePointsController.ResetRoundName();
            inGamePointsController.ResetPoints();
            inGamePointsOverlay.SetActive(false);

            var roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
            var roundOverlayController = roundOverlay.GetComponent<RoundOverlayController>();
            roundOverlayController.HideText();

            var scoreMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.ScoreMenuCanvas, findInactive: true);
            var scoreMenuController = scoreMenu.GetComponent<ScoreMenuController>();
            this.telemetryService.LogInfo(scoreMenuController?.ToString());
            scoreMenuController.DisplayScores(
                args.Data.PlayerPoints,
                args.Data.CurrentRoundIndex,
                args.Data.GetCurrentRound().IsRoundPositive());

            this.stateMachine.AutoPlayCards.Disable();
            args.Data.TricksPlayed = 0;

            this.workflow.SetNextStep(nameof(NextRoundStep));
            this.workflow.WaitForEvent(EventNames.ScoreMenuDismissed);

            return Task.CompletedTask;
        }
    }
}
