namespace Barbu
{
    using System.Linq;
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;
    using Barbu.UI.Controllers;
    using UnityEngine;

    public class CompleteGameStep : IStep<RoundArguments>
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
            this.telemetryService.LogInfo("[RoundWorkflow] [CompleteGame] Executing complete game step...");

            var winningPlayers = args.Data.GetWinningPlayerIds();

            GameObject roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
            var controller = roundOverlay.GetComponent<RoundOverlayController>();
            controller.DisplayWinner(winningPlayers);

            Statistics.IncrementGamesFinished(args.Data.GameType);
            if (winningPlayers.Where(id => id == Constants.PlayerIds.Player1).Any())
            {
                Statistics.IncrementGamesWon(args.Data.GameType);
            }

            this.workflow.SetNextStep(null);
            return Task.CompletedTask;
        }
    }
}
