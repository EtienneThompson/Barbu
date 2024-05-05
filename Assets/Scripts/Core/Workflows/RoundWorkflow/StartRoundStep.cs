namespace Barbu
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models.Workflows;

    public class StartRoundStep : IStep<RoundArguments>
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
            throw new System.NotImplementedException();
        }
    }
}
