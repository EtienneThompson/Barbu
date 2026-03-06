namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Linq;
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using UnityEngine;

    public class CompleteGameStep : IStep<RoundArguments>
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
            this.telemetryService.LogInfo("[RoundWorkflow] [CompleteGame] Executing complete game step...");

            var winningPlayers = args.Data.GetWinningPlayerIds();

            var roundOverlay = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.RoundOverlay, findInactive: true);
            var controller = roundOverlay.GetComponent<RoundOverlayController>();
            controller.DisplayWinner(winningPlayers);

            Statistics.IncrementGamesFinished(args.Data.GameType);
            if (winningPlayers.Where(id => id == Constants.PlayerIds.Player1).Any())
            {
                Statistics.IncrementGamesWon(args.Data.GameType);
            }

            this.workflow.SetNextStep(nameof(ShowAdvertisementStep));
            this.workflow.WaitForEvent(EventNames.WinnerAnimationFinished);
            return Task.CompletedTask;
        }
    }
}
