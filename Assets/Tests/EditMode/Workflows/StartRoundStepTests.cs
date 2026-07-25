namespace Barbu.Tests.EditMode.Workflows
{
    using System.Threading.Tasks;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Core.Workflows.RoundWorkflow;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    // Only the "all piles already played" branch is covered here, which returns
    // before reaching this.playTrickWorkflowFactory.Create(...) - actually invoking
    // that factory needs a real Zenject container to resolve PlayTrickWorkflow's
    // own step factories, which is out of scope for an EditMode unit test (see
    // PlayTrickWorkflowTests/WorkflowFactoryTests for the same boundary).
    public class StartRoundStepTests
    {
        private FakeInGamePointsController inGamePointsController;
        private FakeRoundOverlayController roundOverlayController;
        private FakeStateMachine stateMachine;
        private FakeWorkflow workflow;
        private StartRoundStep step;

        [SetUp]
        public void SetUp()
        {
            this.inGamePointsController = new FakeInGamePointsController();
            this.roundOverlayController = new FakeRoundOverlayController();
            this.stateMachine = new FakeStateMachine();
            this.workflow = new FakeWorkflow();
            this.step = new StartRoundStep(
                this.inGamePointsController, new PlayTrickWorkflow.Factory(), this.roundOverlayController, this.stateMachine, new FakeTelemetryService());
        }

        private StepArguments<RoundArguments> BuildArgs() =>
            new StepArguments<RoundArguments>(this.workflow)
            {
                Data = new RoundArguments { TricksPlayed = Constants.NumPilesPerRound },
            };

        [Test]
        public async Task InvokeAsync_HidesRoundOverlay()
        {
            await this.step.InvokeAsync(BuildArgs());

            CollectionAssert.Contains(this.roundOverlayController.ActiveStates, false);
        }

        [Test]
        public async Task InvokeAsync_ResetsNumCardsPlayed()
        {
            this.stateMachine.IncrementNumCardsPlayed();

            await this.step.InvokeAsync(BuildArgs());

            Assert.AreEqual(0, this.stateMachine.NumCardsPlayed());
        }

        [Test]
        public async Task InvokeAsync_AllPilesPlayed_AdvancesToCleanupRoundStep()
        {
            await this.step.InvokeAsync(BuildArgs());

            Assert.AreEqual(nameof(CleanupRoundStep), this.workflow.NextStepName);
        }
    }
}
