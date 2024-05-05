namespace Barbu.Core.Workflows.PlayTrickWorkflow
{
    using System.Threading.Tasks;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;

    public class ResolveTrickStep : IStep<PlayTrickArguments>
    {
        private IWorkflow parentWorkflow;
        private StateMachine stateMachine;
        private ITelemetryService telemetryService;

        public void Initialize(IWorkflow workflow, StateMachine stateMachine, ITelemetryService telemetryService)
        {
            this.parentWorkflow = workflow;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<PlayTrickArguments> args)
        {
            this.telemetryService.LogInfo("[PlayTrickWorkflow] Resolving Pile...");
            var highestCard = args.Data.currentPile.GetHighestCard();
            args.Data.currentPile.StartPileResolution(highestCard.playerId);
            this.stateMachine.SetHighestRank(0);
            this.parentWorkflow.SetNextStep(null);
            return Task.CompletedTask;
        }
    }
}