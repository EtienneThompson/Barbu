namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.Core.Workflows;

    /// <summary>
    /// Minimal concrete BaseWorkflow&lt;T&gt; so the abstract step-orchestration
    /// logic (sequencing, pausing, validation) can be exercised without any of
    /// the real game's steps or their Card/scene dependencies.
    /// </summary>
    public class TestWorkflow : BaseWorkflow<string>
    {
        private readonly Dictionary<string, IStep<string>> steps;

        public TestWorkflow(
            IEventsController eventsController,
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            Dictionary<string, IStep<string>> steps)
            : base(eventsController, stateMachine, telemetryService)
        {
            this.steps = steps;
        }

        protected override Dictionary<string, IStep<string>> Steps => this.steps;

        public string CurrentStepName => this.currentStepName;

        public bool IsWorkflowPaused => this.IsPaused;

        public int WorkflowEndCount { get; private set; }

        public void SetStartingStep(string stepName)
        {
            this.currentStepName = stepName;
        }

        public void SetData(string data)
        {
            this.Arguments = new StepArguments<string>(this) { Data = data };
        }

        protected override System.Threading.Tasks.Task OnWorkflowEnd()
        {
            this.WorkflowEndCount++;
            return base.OnWorkflowEnd();
        }
    }
}
