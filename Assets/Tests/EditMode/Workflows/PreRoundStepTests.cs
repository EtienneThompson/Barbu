namespace Barbu.Tests.EditMode.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.RoundWorkflow;
    using Barbu.Gameplay.Rounds;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class PreRoundStepTests
    {
        private FakeInGamePointsController inGamePointsController;
        private FakeRoundOverlayController roundOverlayController;
        private FakeWorkflow workflow;
        private PreRoundStep step;

        [SetUp]
        public void SetUp()
        {
            this.inGamePointsController = new FakeInGamePointsController();
            this.roundOverlayController = new FakeRoundOverlayController();
            this.workflow = new FakeWorkflow();
            this.step = new PreRoundStep(this.inGamePointsController, this.roundOverlayController, new FakeTelemetryService());
        }

        private StepArguments<RoundArguments> BuildArgs(GameTypes gameType = GameTypes.Traditional) =>
            new StepArguments<RoundArguments>(this.workflow)
            {
                Data = new RoundArguments
                {
                    GameType = gameType,
                    Rounds = new List<IRound> { new HeartsRound() },
                    CurrentRoundIndex = 0,
                },
            };

        [Test]
        public async Task InvokeAsync_ShowsRoundOverlayWithCurrentRoundNameAndGameType()
        {
            await this.step.InvokeAsync(BuildArgs(GameTypes.Chaos));

            CollectionAssert.Contains(this.roundOverlayController.ActiveStates, true);
            Assert.AreEqual(1, this.roundOverlayController.DisplayedRounds.Count);
            Assert.AreEqual(new HeartsRound().Name, this.roundOverlayController.DisplayedRounds[0].roundName);
            Assert.AreEqual(GameTypes.Chaos, this.roundOverlayController.DisplayedRounds[0].gameType);
        }

        [Test]
        public async Task InvokeAsync_ResetsInGamePointsRoundName()
        {
            await this.step.InvokeAsync(BuildArgs());

            Assert.AreEqual(1, this.inGamePointsController.ResetRoundNameCallCount);
        }

        [Test]
        public async Task InvokeAsync_AdvancesToStartRoundStepAndWaitsForRoundAnimation()
        {
            await this.step.InvokeAsync(BuildArgs());

            Assert.AreEqual(nameof(StartRoundStep), this.workflow.NextStepName);
            CollectionAssert.Contains(this.workflow.EventsWaitedFor, EventNames.RoundAnimationFinished);
        }
    }
}
