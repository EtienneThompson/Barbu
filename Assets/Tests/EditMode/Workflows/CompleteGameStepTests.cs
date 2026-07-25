namespace Barbu.Tests.EditMode.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Core.Events;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.RoundWorkflow;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class CompleteGameStepTests
    {
        private FakeRoundOverlayController roundOverlayController;
        private FakeStatisticsService statisticsService;
        private FakeWorkflow workflow;
        private CompleteGameStep step;

        [SetUp]
        public void SetUp()
        {
            this.roundOverlayController = new FakeRoundOverlayController();
            this.statisticsService = new FakeStatisticsService();
            this.workflow = new FakeWorkflow();
            this.step = new CompleteGameStep(this.roundOverlayController, this.statisticsService, new FakeTelemetryService());
        }

        private static RoundArguments BuildRoundArguments(GameTypes gameType)
        {
            var playerPoints = new Dictionary<string, int[]>
            {
                [Constants.PlayerIds.Player1] = new int[1],
                [Constants.PlayerIds.Player2] = new int[1],
                [Constants.PlayerIds.Player3] = new int[1],
                [Constants.PlayerIds.Player4] = new int[1],
            };

            return new RoundArguments
            {
                GameType = gameType,
                PlayerPoints = playerPoints,
            };
        }

        [Test]
        public async Task InvokeAsync_ShowsWinnerOnRoundOverlay()
        {
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = BuildRoundArguments(GameTypes.Traditional) };

            await this.step.InvokeAsync(args);

            CollectionAssert.Contains(this.roundOverlayController.ActiveStates, true);
            Assert.AreEqual(1, this.roundOverlayController.DisplayedWinners.Count);
        }

        [Test]
        public async Task InvokeAsync_AlwaysIncrementsGamesFinished()
        {
            // Player1 has the minimum (winning) points, so Player1 is the winner here.
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = BuildRoundArguments(GameTypes.Chaos) };

            await this.step.InvokeAsync(args);

            CollectionAssert.AreEqual(new[] { GameTypes.Chaos }, this.statisticsService.GamesFinishedIncrements);
        }

        [Test]
        public async Task InvokeAsync_Player1Wins_IncrementsGamesWon()
        {
            var data = BuildRoundArguments(GameTypes.Traditional);
            data.PlayerPoints[Constants.PlayerIds.Player2][0] = 10;
            data.PlayerPoints[Constants.PlayerIds.Player3][0] = 10;
            data.PlayerPoints[Constants.PlayerIds.Player4][0] = 10;
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = data };

            await this.step.InvokeAsync(args);

            CollectionAssert.AreEqual(new[] { GameTypes.Traditional }, this.statisticsService.GamesWonIncrements);
        }

        [Test]
        public async Task InvokeAsync_Player1DoesNotWin_DoesNotIncrementGamesWon()
        {
            var data = BuildRoundArguments(GameTypes.Traditional);
            data.PlayerPoints[Constants.PlayerIds.Player1][0] = 10;
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = data };

            await this.step.InvokeAsync(args);

            CollectionAssert.IsEmpty(this.statisticsService.GamesWonIncrements);
        }

        [Test]
        public async Task InvokeAsync_AdvancesToShowAdvertisementStepAndWaitsForWinnerAnimation()
        {
            var args = new StepArguments<RoundArguments>(this.workflow) { Data = BuildRoundArguments(GameTypes.Traditional) };

            await this.step.InvokeAsync(args);

            Assert.AreEqual(nameof(ShowAdvertisementStep), this.workflow.NextStepName);
            CollectionAssert.Contains(this.workflow.EventsWaitedFor, EventNames.WinnerAnimationFinished);
        }
    }
}
