namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using Zenject;

    public class CleanupRoundStep : IStep<RoundArguments>
    {
        public class Factory : PlaceholderFactory<CleanupRoundStep>
        {
        }

        private readonly IInGamePointsController inGamePointsController;
        private readonly IGameBoard gameBoard;
        private readonly IRoundOverlayController roundOverlayController;
        private readonly IScoreMenuController scoreMenuController;
        private readonly IStateMachine stateMachine;
        private readonly ITelemetryService telemetryService;

        public CleanupRoundStep(
            IInGamePointsController inGamePointsController,
            IGameBoard gameBoard,
            IRoundOverlayController roundOverlayController,
            IScoreMenuController scoreMenuController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.inGamePointsController = inGamePointsController;
            this.gameBoard = gameBoard;
            this.roundOverlayController = roundOverlayController;
            this.scoreMenuController = scoreMenuController;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [CleanupRound] Executing CleanupRound step...");

            this.gameBoard.CleanupRound();

            this.inGamePointsController.ResetRoundName();
            this.inGamePointsController.ResetPoints();
            this.inGamePointsController.SetActive(false);

            this.roundOverlayController.HideText();

            this.scoreMenuController.DisplayScores(
                args.Data.PlayerPoints,
                args.Data.CurrentRoundIndex,
                args.Data.GetCurrentRound().IsRoundPositive());

            this.stateMachine.SetAutoPlayMode(false);
            args.Data.TricksPlayed = 0;

            args.Workflow.SetNextStep(nameof(NextRoundStep));
            args.Workflow.WaitForEvent(EventNames.ScoreMenuDismissed);

            return Task.CompletedTask;
        }
    }
}
