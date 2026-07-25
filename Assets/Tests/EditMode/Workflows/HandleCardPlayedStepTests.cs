namespace Barbu.Tests.EditMode.Workflows
{
    using System.Collections.Generic;
    using Barbu.Core.Workflows;
    using Barbu.Core.Workflows.PlayTrickWorkflow;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;
    using UnityEngine.TestTools;

    // Drives real Card.Highlight/RemoveHighlight, so cards here are built via
    // CardTestUtils.CreateCardWithRenderer (attaches a MeshRenderer with a
    // built-in-shader material) rather than the plain CreateCard used elsewhere.
    // Renderer.material's getter always logs an Error-level "will leak materials
    // in edit mode" warning when touched outside Play Mode - Unity's test runner
    // fails a test on any unhandled error-level log, so that noise (not a real
    // failure) is suppressed via LogAssert.ignoreFailingMessages (see InvokeStep
    // for why that has to be set from inside the test body). Tests call
    // InvokeAsync().GetAwaiter().GetResult() rather than using async Task test
    // methods, since HandleCardPlayedStep.InvokeAsync always completes
    // synchronously and this keeps the whole test on one straightforward path.
    public class HandleCardPlayedStepTests
    {
        private readonly List<Card> createdCards = new();
        private FakeStateMachine stateMachine;
        private FakeEventsController eventsController;
        private FakeWorkflow workflow;
        private HandleCardPlayedStep step;
        private RecordingGameState[] gameStates;
        private Pile pile;

        [SetUp]
        public void SetUp()
        {
            this.stateMachine = new FakeStateMachine();
            this.eventsController = new FakeEventsController();
            this.workflow = new FakeWorkflow();
            this.step = new HandleCardPlayedStep(this.stateMachine, new FakeTelemetryService());
            this.pile = new Pile(this.stateMachine, this.eventsController);
            this.gameStates = new[]
            {
                new RecordingGameState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), Constants.PlayerIds.Player1, new Hand()),
                new RecordingGameState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), Constants.PlayerIds.Player2, new Hand()),
                new RecordingGameState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), Constants.PlayerIds.Player3, new Hand()),
                new RecordingGameState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), Constants.PlayerIds.Player4, new Hand()),
            };
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

        private Card CreateCard(string suit, string rank)
        {
            var card = CardTestUtils.CreateCardWithRenderer(suit, rank, stateMachine: this.stateMachine, eventsController: this.eventsController);
            this.createdCards.Add(card);
            return card;
        }

        private StepArguments<PlayTrickArguments> BuildArgs(Card playedCard, int currentGameStateIndex = 0) =>
            new StepArguments<PlayTrickArguments>(this.workflow)
            {
                Data = new PlayTrickArguments
                {
                    gameStates = this.gameStates,
                    currentGameStateIndex = currentGameStateIndex,
                    currentPile = this.pile,
                },
                EventData = playedCard,
            };

        private void InvokeStep(StepArguments<PlayTrickArguments> args)
        {
            // Must be set from within the test body, not [SetUp]: Unity's test
            // runner wraps [SetUp] in its own short-lived LogScope that's disposed
            // before the test method runs, so a flag set there doesn't apply here.
            LogAssert.ignoreFailingMessages = true;
            this.step.InvokeAsync(args).GetAwaiter().GetResult();
        }

        [Test]
        public void InvokeAsync_SetsCardNotPlayable()
        {
            this.stateMachine.SetCardPlayable(true);
            var card = this.CreateCard("Heart", "05");

            this.InvokeStep(this.BuildArgs(card));

            Assert.IsFalse(this.stateMachine.IsCardPlayable());
        }

        [Test]
        public void InvokeAsync_CleansUpCurrentPlayersGameState()
        {
            var card = this.CreateCard("Heart", "05");

            this.InvokeStep(this.BuildArgs(card, currentGameStateIndex: 2));

            Assert.AreEqual(1, this.gameStates[2].CleanUpCallCount);
            Assert.AreEqual(0, this.gameStates[0].CleanUpCallCount);
        }

        [Test]
        public void InvokeAsync_AddsPlayedCardToPile()
        {
            var card = this.CreateCard("Heart", "05");

            this.InvokeStep(this.BuildArgs(card));

            Assert.AreEqual(1, this.pile.GetPileSize());
            CollectionAssert.Contains(this.pile.GetCards(), card);
        }

        [Test]
        public void InvokeAsync_IncrementsCurrentGameStateIndex()
        {
            var card = this.CreateCard("Heart", "05");
            var args = this.BuildArgs(card, currentGameStateIndex: 1);

            this.InvokeStep(args);

            Assert.AreEqual(2, args.Data.currentGameStateIndex);
        }

        [Test]
        public void InvokeAsync_SetsHighestRankToNewlyPlayedCard()
        {
            var card = this.CreateCard("Heart", "09");

            this.InvokeStep(this.BuildArgs(card));

            Assert.AreEqual(9, this.stateMachine.GetHighestRankedCard());
        }

        [Test]
        public void InvokeAsync_PileNotYetFull_AdvancesToPlayCardStep()
        {
            var card = this.CreateCard("Heart", "05");

            this.InvokeStep(this.BuildArgs(card));

            Assert.AreEqual(nameof(PlayCardStep), this.workflow.NextStepName);
        }

        [Test]
        public void InvokeAsync_FourthCardCompletesPile_AdvancesToResolveTrickStep()
        {
            for (int i = 0; i < 3; i++)
            {
                this.pile.AddCardToPile(this.CreateCard("Club", "02"));
            }
            var fourthCard = this.CreateCard("Heart", "05");

            this.InvokeStep(this.BuildArgs(fourthCard, currentGameStateIndex: 3));

            Assert.AreEqual(nameof(ResolveTrickStep), this.workflow.NextStepName);
        }
    }
}
