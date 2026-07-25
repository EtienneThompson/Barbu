namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using Zenject;

    public class ResolveTrickStep : IStep<PlayTrickArguments>
    {
        public class Factory : PlaceholderFactory<ResolveTrickStep>
        {
        }

        private readonly IInGamePointsController inGamePointsController;
        private readonly IStateMachine stateMachine;
        private readonly ITelemetryService telemetryService;

        public ResolveTrickStep(IInGamePointsController inGamePointsController, IStateMachine stateMachine, ITelemetryService telemetryService)
        {
            this.inGamePointsController = inGamePointsController;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<PlayTrickArguments> args)
        {
            this.telemetryService.LogInfo("[PlayTrickWorkflow] Resolving Pile...");
            var highestCard = args.Data.currentPile.GetHighestCard();

            this.telemetryService.LogInfo("[PlayTrickWorkflow] Handling end of workflow...");
            var pointsInPile = args.Data.Round.CalculatePointsInPile(args.Data.currentPile);
            var playerId = highestCard.playerId;

            this.telemetryService.LogInfo($"[PlayTrickWorkflow] Winning player: {playerId}");
            this.inGamePointsController.UpdatePlayerPoints(playerId, pointsInPile);

            args.Data.PlayerPoints[playerId][args.Data.RoundNumber] += pointsInPile;

            args.Data.currentPile.StartPileResolution(highestCard.playerId);
            this.stateMachine.SetHighestRank(0);
            args.Workflow.SetNextStep(null);
            return Task.CompletedTask;
        }
    }
}
