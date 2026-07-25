namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Linq;
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using Zenject;

    public class CompleteGameStep : IStep<RoundArguments>
    {
        public class Factory : PlaceholderFactory<CompleteGameStep>
        {
        }

        private readonly IRoundOverlayController roundOverlayController;
        private readonly IStatisticsService statisticsService;
        private readonly ITelemetryService telemetryService;

        public CompleteGameStep(
            IRoundOverlayController roundOverlayController,
            IStatisticsService statisticsService,
            ITelemetryService telemetryService)
        {
            this.roundOverlayController = roundOverlayController;
            this.statisticsService = statisticsService;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [CompleteGame] Executing complete game step...");

            var winningPlayers = args.Data.GetWinningPlayerIds();

            this.roundOverlayController.SetActive(true);
            this.roundOverlayController.DisplayWinner(winningPlayers);

            this.statisticsService.IncrementGamesFinished(args.Data.GameType);
            if (winningPlayers.Where(id => id == Constants.PlayerIds.Player1).Any())
            {
                this.statisticsService.IncrementGamesWon(args.Data.GameType);
            }

            args.Workflow.SetNextStep(nameof(ShowAdvertisementStep));
            args.Workflow.WaitForEvent(EventNames.WinnerAnimationFinished);
            return Task.CompletedTask;
        }
    }
}
