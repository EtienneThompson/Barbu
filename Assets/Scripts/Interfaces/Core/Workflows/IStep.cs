namespace Barbu.Interfaces.Core.Workflows
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Models.Workflows;

    public interface IStep<T>
    {
        void Initialize(IWorkflow workflow, StateMachine stateMachine, ITelemetryService telemetryService);

        Task InvokeAsync(StepArguments<T> args);
    }
}