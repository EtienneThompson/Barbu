namespace Barbu
{
    using System;
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;
    using Barbu.UI.Controllers;
    using UnityEngine;

    public class StartRoundStep : IStep<RoundArguments>
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
            this.telemetryService.LogInfo("[RoundWorkflow] [StartRoundStep] Executing StartRound step...");
            this.stateMachine.ResetNumCardsPlayed();

            if (args.Data.GetCurrentRound().IsRoundOver(args.Data.CurrentRoundIndex, args.Data.PlayerPoints, args.Data.TricksPlayed))
            {
                this.telemetryService.LogInfo("[Round Workflow] [StartRound] Round is over, moving to cleanup step...");
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

            this.telemetryService.LogInfo($"[Round Workflow] [StartRound] Setting starting player: {startingPlayerId}");
            args.Data.PlayTrickWorkflow = new PlayTrickWorkflow(
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
            this.workflow.WaitForEvent(Models.EventNames.PileResolved);

            return Task.CompletedTask;
        }
    }
}
