namespace Barbu.Core.Workflows
{
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;

    public interface IStep<T>
    {
        void Initialize(
            IWorkflow workflow,
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService);

        Task InvokeAsync(StepArguments<T> args);
    }
}