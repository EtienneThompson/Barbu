namespace Barbu
{
    using System.Threading.Tasks;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;

    public class HandleCardPlayedStep : IStep<PlayTrickArguments>
    {
        private IWorkflow workflow;
        private ITelemetryService telemetryService;

        public void Initialize(IWorkflow workflow, ITelemetryService telemetryService)
        {
            this.workflow = workflow;
            this.telemetryService = telemetryService;
        }

        public Task InvokeAsync(StepArguments<PlayTrickArguments> args)
        {
            this.telemetryService.LogInfo("[PlayTrickWorkflow] Handling played card...");
            this.workflow.SetNextStep(nameof(PlayCardStep));
            return Task.CompletedTask;
        }
    }
}
