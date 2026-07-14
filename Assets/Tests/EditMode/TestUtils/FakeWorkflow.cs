namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Workflows;

    /// <summary>
    /// IWorkflow double for testing an IStep in isolation, without a real
    /// BaseWorkflow driving it. Records SetNextStep/Pause/WaitForEvent calls so
    /// tests can assert on how a step tries to steer its workflow.
    /// </summary>
    public class FakeWorkflow : IWorkflow
    {
        public string NextStepName { get; private set; } = "Unset";

        public int PauseCallCount { get; private set; }

        public int StartAsyncCallCount { get; private set; }

        public List<EventNames> EventsWaitedFor { get; } = new();

        public List<EventNames> EventsWaitedForWithData { get; } = new();

        public Task StartAsync()
        {
            this.StartAsyncCallCount++;
            return Task.CompletedTask;
        }

        public void Pause(EventNames eventName = EventNames.PauseGame)
        {
            this.PauseCallCount++;
        }

        public void WaitForEvent(EventNames eventName)
        {
            this.EventsWaitedFor.Add(eventName);
        }

        public void WaitForEventWithData(EventNames eventName)
        {
            this.EventsWaitedForWithData.Add(eventName);
        }

        public void SetNextStep(string stepName)
        {
            this.NextStepName = stepName;
        }
    }
}
