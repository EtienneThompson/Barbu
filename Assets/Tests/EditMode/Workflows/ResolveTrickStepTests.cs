namespace Barbu.Tests.EditMode.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class ResolveTrickStepTests
    {
        private readonly List<Card> createdCards = new();
        private FakeStateMachine stateMachine;
        private FakeEventsController eventsController;
        private FakeInGamePointsController inGamePointsController;
        private FakeWorkflow workflow;
        private ResolveTrickStep step;
        private Pile pile;

        [SetUp]
        public void SetUp()
        {
            this.stateMachine = new FakeStateMachine();
            this.eventsController = new FakeEventsController();
            this.inGamePointsController = new FakeInGamePointsController();
            this.workflow = new FakeWorkflow();
            this.step = new ResolveTrickStep(this.inGamePointsController, this.stateMachine, new FakeTelemetryService());
            this.pile = new Pile(this.stateMachine, this.eventsController);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var card in this.createdCards)
            {
                UnityEngine.Object.DestroyImmediate(card.gameObject);
            }

            this.createdCards.Clear();
        }

        private void AddCardToPile(string suit, string rank, string playerId)
        {
            var card = CardTestUtils.CreateCard(suit, rank, playerId, this.stateMachine, this.eventsController);
            this.createdCards.Add(card);
            this.pile.AddCardToPile(card);
        }

        private StepArguments<PlayTrickArguments> BuildArgs(IRound round, int roundNumber = 0) =>
            new StepArguments<PlayTrickArguments>(this.workflow)
            {
                Data = new PlayTrickArguments
                {
                    currentPile = this.pile,
                    Round = round,
                    RoundNumber = roundNumber,
                    PlayerPoints = new Dictionary<string, int[]>
                    {
                        [Constants.PlayerIds.Player1] = new int[roundNumber + 1],
                        [Constants.PlayerIds.Player2] = new int[roundNumber + 1],
                        [Constants.PlayerIds.Player3] = new int[roundNumber + 1],
                        [Constants.PlayerIds.Player4] = new int[roundNumber + 1],
                    },
                },
            };

        [Test]
        public async Task InvokeAsync_AwardsPileValueToWinningPlayer()
        {
            this.AddCardToPile("Heart", "05", Constants.PlayerIds.Player2);
            var args = this.BuildArgs(new HeartsRound());

            await this.step.InvokeAsync(args);

            Assert.AreEqual(5, this.inGamePointsController.PlayerPoints[Constants.PlayerIds.Player2]);
            Assert.AreEqual(5, args.Data.PlayerPoints[Constants.PlayerIds.Player2][0]);
        }

        [Test]
        public async Task InvokeAsync_ZeroPointPile_StillNotifiesWinnerWithZero()
        {
            this.AddCardToPile("Club", "05", Constants.PlayerIds.Player3);
            var args = this.BuildArgs(new HeartsRound());

            await this.step.InvokeAsync(args);

            Assert.AreEqual(0, this.inGamePointsController.PlayerPoints[Constants.PlayerIds.Player3]);
        }

        [Test]
        public async Task InvokeAsync_ResetsHighestRankAndEndsWorkflow()
        {
            this.AddCardToPile("Heart", "05", Constants.PlayerIds.Player1);
            this.stateMachine.SetHighestRank(5);
            var args = this.BuildArgs(new HeartsRound());

            await this.step.InvokeAsync(args);

            Assert.AreEqual(0, this.stateMachine.GetHighestRankedCard());
            Assert.IsNull(this.workflow.NextStepName);
        }
    }
}
