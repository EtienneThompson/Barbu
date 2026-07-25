namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System;
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.UI.Controllers;
    using Zenject;

    public class StartRoundStep : IStep<RoundArguments>
    {
        public class Factory : PlaceholderFactory<StartRoundStep>
        {
        }

        private readonly IInGamePointsController inGamePointsController;
        private readonly PlayTrickWorkflow.Factory playTrickWorkflowFactory;
        private readonly IRoundOverlayController roundOverlayController;
        private readonly IStateMachine stateMachine;
        private readonly ITelemetryService telemetryService;

        public StartRoundStep(
            IInGamePointsController inGamePointsController,
            PlayTrickWorkflow.Factory playTrickWorkflowFactory,
            IRoundOverlayController roundOverlayController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.inGamePointsController = inGamePointsController;
            this.playTrickWorkflowFactory = playTrickWorkflowFactory;
            this.roundOverlayController = roundOverlayController;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [StartRoundStep] Executing StartRound step...");
            this.roundOverlayController.SetActive(false);
            this.stateMachine.ResetNumCardsPlayed();

            if (args.Data.TricksPlayed == Constants.NumPilesPerRound)
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [StartRound] All piles played, moving to cleanup step...");
                args.Workflow.SetNextStep(nameof(CleanupRoundStep));
                return Task.CompletedTask;
            }

            var startingPlayerId = args.Data.CurrentRoundIndex % 4;
            if (args.Data.PlayTrickWorkflow != null)
            {
                startingPlayerId = Int32.Parse(args.Data.PlayTrickWorkflow.GetWinningPlayerId()) - 1;
                args.Data.PlayTrickWorkflow.Dispose();
                args.Data.PlayTrickWorkflow = null;
            }

            if (args.Data.GetCurrentRound().IsRoundOver(args.Data.CurrentRoundIndex, args.Data.PlayerPoints, args.Data.TricksPlayed))
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [StartRound] Round is over, auto playing remaining cards.");
                this.stateMachine.SetAutoPlayMode(true);
                this.roundOverlayController.SetActive(true);
                this.roundOverlayController.ShowRoundOverMessage();
            }

            this.telemetryService.LogInfo($"[RoundWorkflow] [StartRound] Setting starting player: {startingPlayerId}");
            args.Data.PlayTrickWorkflow = this.playTrickWorkflowFactory.Create(
                args.Data.GetCurrentRound(),
                args.Data.PlayerPoints,
                args.Data.Hands,
                startingPlayerId,
                args.Data.CurrentRoundIndex);
            Task _ = args.Data.PlayTrickWorkflow.StartAsync();
            args.Data.TricksPlayed++;

            this.inGamePointsController.SetRoundName(args.Data.GetCurrentRound().Name);

            args.Workflow.SetNextStep(nameof(StartRoundStep));
            args.Workflow.WaitForEvent(EventNames.PileResolved);

            return Task.CompletedTask;
        }
    }
}
