namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Core.Workflows;

    /// <summary>IStep double that records invocations and advances to whatever step name it's told to.</summary>
    public class RecordingStep : IStep<string>
    {
        public List<string> InvocationsWithData { get; } = new();

        public string NextStepName { get; set; }

        public Task InvokeAsync(StepArguments<string> args)
        {
            this.InvocationsWithData.Add(args.Data);
            args.Workflow.SetNextStep(this.NextStepName);
            return Task.CompletedTask;
        }
    }
}
