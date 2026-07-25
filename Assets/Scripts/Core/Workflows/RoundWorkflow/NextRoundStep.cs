namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using Zenject;

    public class NextRoundStep : IStep<RoundArguments>
    {
        public class Factory : PlaceholderFactory<NextRoundStep>
        {
        }

        private readonly IInGamePointsController inGamePointsController;
        private readonly ITelemetryService telemetryService;

        public NextRoundStep(IInGamePointsController inGamePointsController, ITelemetryService telemetryService)
        {
            this.inGamePointsController = inGamePointsController;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] [NextRound] Executing next round step...");
            this.inGamePointsController.SetActive(true);

            if (args.Data.CurrentRoundIndex + 1 == args.Data.Rounds.Count)
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [NextRound] Rounds are complete, moving to complete game step...");
                args.Workflow.SetNextStep(nameof(CompleteGameStep));
            }
            else
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [NextRound] Configuring next round...");
                args.Data.PlayTrickWorkflow.Dispose();
                args.Data.PlayTrickWorkflow = null;
                args.Data.CurrentRoundIndex++;
                args.Workflow.SetNextStep(nameof(SetupRoundStep));
            }

            return Task.CompletedTask;
        }
    }
}
