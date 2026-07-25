namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.UI.Controllers;
    using Zenject;

    public class PreRoundStep : IStep<RoundArguments>
    {
        public class Factory : PlaceholderFactory<PreRoundStep>
        {
        }

        private readonly IInGamePointsController inGamePointsController;
        private readonly IRoundOverlayController roundOverlayController;
        private readonly ITelemetryService telemetryService;

        public PreRoundStep(
            IInGamePointsController inGamePointsController,
            IRoundOverlayController roundOverlayController,
            ITelemetryService telemetryService)
        {
            this.inGamePointsController = inGamePointsController;
            this.roundOverlayController = roundOverlayController;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<RoundArguments> args)
        {
            this.telemetryService.LogInfo("[RoundWorkflow] Executing PreRound step...");
            this.roundOverlayController.SetActive(true);
            this.roundOverlayController.DisplayRound(args.Data.GetCurrentRound().Name, args.Data.GameType);

            this.inGamePointsController.ResetRoundName();

            args.Workflow.SetNextStep(nameof(StartRoundStep));
            args.Workflow.WaitForEvent(EventNames.RoundAnimationFinished);

            return Task.CompletedTask;
        }
    }
}
