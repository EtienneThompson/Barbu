namespace Barbu.Tests.EditMode.Workflows
{
    using System.Threading.Tasks;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.RoundWorkflow;
    using Barbu.Gameplay;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class SetupRoundStepTests
    {
        private FakeGameBoard gameBoard;
        private FakeWorkflow workflow;
        private SetupRoundStep step;

        [SetUp]
        public void SetUp()
        {
            this.gameBoard = new FakeGameBoard();
            this.workflow = new FakeWorkflow();
            this.step = new SetupRoundStep(this.gameBoard, new FakeTelemetryService());
        }

        [Test]
        public async Task InvokeAsync_StoresHandsReturnedByGameBoard()
        {
            var hands = new[] { new Hand(), new Hand(), new Hand(), new Hand() };
            this.gameBoard.HandsToReturn = hands;
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = new RoundArguments() };

            await this.step.InvokeAsync(args);

            Assert.AreSame(hands, args.Data.Hands);
            Assert.AreEqual(1, this.gameBoard.SetupRoundCallCount);
        }

        [Test]
        public async Task InvokeAsync_AdvancesToPreRoundStep()
        {
            await this.step.InvokeAsync(new StepArguments<RoundArguments>(this.workflow) { Data = new RoundArguments() });

            Assert.AreEqual(nameof(PreRoundStep), this.workflow.NextStepName);
        }
    }
}
