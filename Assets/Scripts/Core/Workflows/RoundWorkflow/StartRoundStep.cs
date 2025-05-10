namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System;
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.UI.Controllers;
    using UnityEngine;

    public class StartRoundStep : IStep<RoundArguments>
    {
        private IWorkflow workflow;
        private IEventsController eventsController;
        private IStateMachine stateMachine;
        private ITelemetryService telemetryService;

        public void Initialize(
            IWorkflow workflow,
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.workflow = workflow;
            this.eventsController = eventsController;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [StartRoundStep] Executing StartRound step...");
            this.stateMachine.ResetNumCardsPlayed();

            if (args.Data.TricksPlayed == Constants.NumPilesPerRound)
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [StartRound] All piles played, moving to cleanup step...");
                this.workflow.SetNextStep(nameof(CleanupRoundStep));
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
                var roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
                var roundOverlayController = roundOverlay.GetComponent<RoundOverlayController>();
                roundOverlayController.ShowRoundOverMessage();
            }

            this.telemetryService.LogInfo($"[RoundWorkflow] [StartRound] Setting starting player: {startingPlayerId}");
            args.Data.PlayTrickWorkflow = new PlayTrickWorkflow(
                this.eventsController,
                this.stateMachine,
                this.telemetryService,
                args.Data.ComputerStateFactory,
                args.Data.GetCurrentRound(),
                args.Data.PlayerPoints,
                args.Data.Hands,
                startingPlayerId,
                args.Data.CurrentRoundIndex);
            Task _ = args.Data.PlayTrickWorkflow.StartAsync();
            args.Data.TricksPlayed++;

            var inGamePointsOverlay = GameObject.Find(Constants.GameObjects.InGamePoints);
            var inGamePointsController = inGamePointsOverlay.GetComponent<InGamePointsController>();
            inGamePointsController.SetRoundName(args.Data.GetCurrentRound().Name);

            this.workflow.SetNextStep(nameof(StartRoundStep));
            this.workflow.WaitForEvent(EventNames.PileResolved);

            return Task.CompletedTask;
        }
    }
}
