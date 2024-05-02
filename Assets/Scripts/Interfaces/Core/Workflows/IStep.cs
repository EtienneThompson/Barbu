namespace Barbu.Interfaces.Core.Workflows
{
    using System.Threading.Tasks;
    using Barbu.Models.Workflows;

    public interface IStep<T>
    {
        void Initialize(IWorkflow workflow, ITelemetryService telemetryService);

        Task InvokeAsync(StepArguments<T> args);
    }
}