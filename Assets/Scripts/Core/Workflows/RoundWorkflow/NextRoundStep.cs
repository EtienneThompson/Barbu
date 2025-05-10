namespace Barbu.Core.Workflows.RoundWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;

    public class NextRoundStep : IStep<RoundArguments>
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
            this.telemetryService.LogInfo("[RoundWorkflow] [NextRound] Executing next round step...");
            var inGamePointsOverlay = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.InGamePoints, findInactive: true);
            inGamePointsOverlay.SetActive(true);

            if (args.Data.CurrentRoundIndex + 1 == args.Data.Rounds.Count)
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [NextRound] Rounds are complete, moving to complete game step...");
                this.workflow.SetNextStep(nameof(CompleteGameStep));
            }
            else
            {
                this.telemetryService.LogInfo("[RoundWorkflow] [NextRound] Configuring next round...");
                args.Data.PlayTrickWorkflow.Dispose();
                args.Data.PlayTrickWorkflow = null;
                args.Data.CurrentRoundIndex++;
                this.workflow.SetNextStep(nameof(SetupRoundStep));
            }

            return Task.CompletedTask;
        }
    }
}
