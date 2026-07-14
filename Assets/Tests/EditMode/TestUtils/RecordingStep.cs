namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.Core.Workflows;

    /// <summary>IStep double that records invocations and advances to whatever step name it's told to.</summary>
    public class RecordingStep : IStep<string>
    {
        public List<string> InvocationsWithData { get; } = new();

        public string NextStepName { get; set; }

        public IWorkflow Workflow { get; private set; }

        public void Initialize(
            IWorkflow workflow,
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService)
        {
            this.Workflow = workflow;
        }

        public Task InvokeAsync(StepArguments<string> args)
        {
            this.InvocationsWithData.Add(args.Data);
            this.Workflow.SetNextStep(this.NextStepName);
            return Task.CompletedTask;
        }
    }
}
