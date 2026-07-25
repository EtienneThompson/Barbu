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

    public class CleanupRoundStepTests
    {
        private FakeInGamePointsController inGamePointsController;
        private FakeGameBoard gameBoard;
        private FakeRoundOverlayController roundOverlayController;
        private FakeScoreMenuController scoreMenuController;
        private FakeStateMachine stateMachine;
        private FakeWorkflow workflow;
        private CleanupRoundStep step;

        [SetUp]
        public void SetUp()
        {
            this.inGamePointsController = new FakeInGamePointsController();
            this.gameBoard = new FakeGameBoard();
            this.roundOverlayController = new FakeRoundOverlayController();
            this.scoreMenuController = new FakeScoreMenuController();
            this.stateMachine = new FakeStateMachine();
            this.workflow = new FakeWorkflow();
            this.step = new CleanupRoundStep(
                this.inGamePointsController, this.gameBoard, this.roundOverlayController, this.scoreMenuController, this.stateMachine, new FakeTelemetryService());
        }

        private static Dictionary<string, int[]> BuildPlayerPoints() =>
            new Dictionary<string, int[]>
            {
                [Constants.PlayerIds.Player1] = new int[1],
                [Constants.PlayerIds.Player2] = new int[1],
                [Constants.PlayerIds.Player3] = new int[1],
                [Constants.PlayerIds.Player4] = new int[1],
            };

        private StepArguments<RoundArguments> BuildArgs(Dictionary<string, int[]> playerPoints, int currentRoundIndex = 0) =>
            new StepArguments<RoundArguments>(this.workflow)
            {
                Data = new RoundArguments
                {
                    Rounds = new List<IRound> { new HeartsRound() },
                    CurrentRoundIndex = currentRoundIndex,
                    PlayerPoints = playerPoints,
                },
            };

        [Test]
        public async Task InvokeAsync_CleansUpGameBoardAndInGamePointsOverlay()
        {
            await this.step.InvokeAsync(BuildArgs(BuildPlayerPoints()));

            Assert.AreEqual(1, this.gameBoard.CleanupRoundCallCount);
            Assert.AreEqual(1, this.inGamePointsController.ResetRoundNameCallCount);
            Assert.AreEqual(1, this.inGamePointsController.ResetPointsCallCount);
            CollectionAssert.Contains(this.inGamePointsController.ActiveStates, false);
        }

        [Test]
        public async Task InvokeAsync_HidesRoundOverlayText()
        {
            await this.step.InvokeAsync(BuildArgs(BuildPlayerPoints()));

            Assert.AreEqual(1, this.roundOverlayController.HideTextCallCount);
        }

        [Test]
        public async Task InvokeAsync_DisplaysScoresForCurrentRound()
        {
            var playerPoints = BuildPlayerPoints();

            await this.step.InvokeAsync(BuildArgs(playerPoints, currentRoundIndex: 0));

            Assert.AreEqual(1, this.scoreMenuController.DisplayScoresCallCount);
            Assert.AreSame(playerPoints, this.scoreMenuController.LastScores);
            Assert.AreEqual(0, this.scoreMenuController.LastCurrentRoundIndex);
        }

        [Test]
        public async Task InvokeAsync_ResetsAutoPlayModeAndTricksPlayed()
        {
            this.stateMachine.SetAutoPlayMode(true);
            var args = BuildArgs(BuildPlayerPoints());
            args.Data.TricksPlayed = 13;

            await this.step.InvokeAsync(args);

            Assert.IsFalse(this.stateMachine.IsAutoPlayMode());
            Assert.AreEqual(0, args.Data.TricksPlayed);
        }

        [Test]
        public async Task InvokeAsync_AdvancesToNextRoundStepAndWaitsForScoreMenuDismissed()
        {
            await this.step.InvokeAsync(BuildArgs(BuildPlayerPoints()));

            Assert.AreEqual(nameof(NextRoundStep), this.workflow.NextStepName);
            CollectionAssert.Contains(this.workflow.EventsWaitedFor, EventNames.ScoreMenuDismissed);
        }
    }
}
