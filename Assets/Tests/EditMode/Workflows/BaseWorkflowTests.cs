namespace Barbu.Tests.EditMode.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Core;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using Barbu.Core.Workflows;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class BaseWorkflowTests
    {
        // Calls workflow.Pause() from inside a step, to distinguish "explicitly
        // paused" (workflow.IsPaused) from "game menu is open"
        // (stateMachine.IsGamePaused()) - BaseWorkflow treats these differently.
        private class PausingStep : IStep<string>
        {
            private IWorkflow workflow;

            public string NextStepName { get; set; }

            public void Initialize(IWorkflow workflow, IEventsController eventsController, IStateMachine stateMachine, ITelemetryService telemetryService)
            {
                this.workflow = workflow;
            }

            public Task InvokeAsync(StepArguments<string> args)
            {
                this.workflow.Pause();
                this.workflow.SetNextStep(this.NextStepName);
                return Task.CompletedTask;
            }
        }

        private FakeEventsController eventsController;
        private FakeStateMachine stateMachine;

        [SetUp]
        public void SetUp()
        {
            this.eventsController = new FakeEventsController();
            this.stateMachine = new FakeStateMachine();
        }

        [Test]
        public void Constructor_SubscribesToPauseAndResumeEvents()
        {
            _ = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), new Dictionary<string, IStep<string>>());

            CollectionAssert.Contains(this.eventsController.SubscribedEvents, EventNames.PauseGame);
            CollectionAssert.Contains(this.eventsController.SubscribedEvents, EventNames.ResumeGame);
        }

        [Test]
        public void Dispose_UnsubscribesFromPauseAndResumeEvents()
        {
            var workflow = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), new Dictionary<string, IStep<string>>());

            workflow.Dispose();

            CollectionAssert.Contains(this.eventsController.UnsubscribedEvents, EventNames.PauseGame);
            CollectionAssert.Contains(this.eventsController.UnsubscribedEvents, EventNames.ResumeGame);
        }

        [Test]
        public void Pause_SetsIsPausedTrue()
        {
            var workflow = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), new Dictionary<string, IStep<string>>());

            workflow.Pause();

            Assert.IsTrue(workflow.IsWorkflowPaused);
        }

        [Test]
        public void SetNextStep_UnknownStepName_ThrowsArgumentException()
        {
            var workflow = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), new Dictionary<string, IStep<string>>());

            Assert.Throws<ArgumentException>(() => workflow.SetNextStep("DoesNotExist"));
        }

        [Test]
        public void StartAsync_NoStartingStepSet_Throws()
        {
            var workflow = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), new Dictionary<string, IStep<string>>());
            workflow.SetData("payload");

            Assert.ThrowsAsync<InvalidOperationException>(async () => await workflow.StartAsync());
        }

        [Test]
        public void StartAsync_StartingStepNotRegistered_Throws()
        {
            var workflow = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), new Dictionary<string, IStep<string>>());
            workflow.SetData("payload");
            workflow.SetStartingStep("Bogus"); // bypasses SetNextStep's own validation

            Assert.ThrowsAsync<InvalidOperationException>(async () => await workflow.StartAsync());
        }

        [Test]
        public async Task StartAsync_RunsStepsInSequenceUntilNextStepIsNull()
        {
            var stepA = new RecordingStep { NextStepName = "B" };
            var stepB = new RecordingStep { NextStepName = null };
            var steps = new Dictionary<string, IStep<string>> { ["A"] = stepA, ["B"] = stepB };
            var workflow = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), steps);
            workflow.SetData("payload");
            workflow.SetStartingStep("A");

            await workflow.StartAsync();

            CollectionAssert.AreEqual(new[] { "payload" }, stepA.InvocationsWithData);
            CollectionAssert.AreEqual(new[] { "payload" }, stepB.InvocationsWithData);
            Assert.IsNull(workflow.CurrentStepName);
            Assert.AreEqual(1, workflow.WorkflowEndCount);
        }

        [Test]
        public async Task StartAsync_GamePausedMidWorkflow_StopsAfterCurrentStep()
        {
            var stepA = new RecordingStep { NextStepName = "B" };
            var stepB = new RecordingStep { NextStepName = null };
            var steps = new Dictionary<string, IStep<string>> { ["A"] = stepA, ["B"] = stepB };
            var workflow = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), steps);
            workflow.SetData("payload");
            workflow.SetStartingStep("A");
            this.stateMachine.SetMenuOpen(true); // IsGamePaused() == true

            await workflow.StartAsync();

            CollectionAssert.AreEqual(new[] { "payload" }, stepA.InvocationsWithData);
            CollectionAssert.IsEmpty(stepB.InvocationsWithData);
            Assert.AreEqual("B", workflow.CurrentStepName); // set by step A, but never consumed
        }

        [Test]
        public async Task StartAsync_PausedExplicitlyByAStep_DoesNotRunWorkflowEnd()
        {
            var stepA = new PausingStep { NextStepName = "B" };
            var stepB = new RecordingStep { NextStepName = null };
            var steps = new Dictionary<string, IStep<string>> { ["A"] = stepA, ["B"] = stepB };
            var workflow = new TestWorkflow(this.eventsController, this.stateMachine, new FakeTelemetryService(), steps);
            workflow.SetData("payload");
            workflow.SetStartingStep("A");

            await workflow.StartAsync();

            CollectionAssert.IsEmpty(stepB.InvocationsWithData);
            Assert.AreEqual(0, workflow.WorkflowEndCount);
            Assert.IsTrue(workflow.IsWorkflowPaused);
        }
    }
}
