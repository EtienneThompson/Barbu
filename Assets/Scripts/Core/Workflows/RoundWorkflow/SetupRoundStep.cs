namespace Barbu.Core.Workflows.RoundWorkflow
{
    using Barbu.Core.Telemetry;
    using System.Threading.Tasks;
    using Zenject;

    public class SetupRoundStep : IStep<RoundArguments>
    {
        public class Factory : PlaceholderFactory<SetupRoundStep>
        {
        }

        private readonly IGameBoard gameBoard;
        private readonly ITelemetryService telemetryService;

        public SetupRoundStep(IGameBoard gameBoard, ITelemetryService telemetryService)
        {
            this.gameBoard = gameBoard;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [SetupRound] Executing setup round step...");

            var hands = this.gameBoard.SetupRound();
            args.Data.Hands = hands;

            args.Workflow.SetNextStep(nameof(PreRoundStep));
            return Task.CompletedTask;
        }
    }
}
