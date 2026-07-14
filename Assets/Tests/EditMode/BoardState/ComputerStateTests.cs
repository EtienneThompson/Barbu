namespace Barbu.Tests.EditMode.BoardState
{
    using System;
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    // ComputerState.Start() relies on the real Card.PlayCard(), which calls
    // MonoBehaviour.StartCoroutine. That's untested territory for EditMode
    // tests (coroutines don't get pumped outside Play Mode) - these tests only
    // assert the synchronous side effects PlayCard performs before starting its
    // coroutine (state flips to Played, stateMachine counters update).
    public class ComputerStateTests
    {
        private readonly List<Card> createdCards = new();
        private FakeStateMachine stateMachine;

        [SetUp]
        public void SetUp()
        {
            this.stateMachine = new FakeStateMachine();
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

        [Test]
        public void Start_PlaysFirstWaitingCardInStartingSuit()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Club", "02"), ("Heart", "05"), ("Heart", "09"),
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"), ("Club", "13"));
            var state = new ComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            state.Start();

            var cards = hand.GetHand();
            Assert.AreEqual(Card.CardState.Played, cards[1].state); // Heart5, first in suit
            Assert.AreEqual(Card.CardState.Waiting, cards[2].state); // Heart9, untouched
            Assert.AreEqual(1, this.stateMachine.NumCardsPlayed());
            Assert.IsFalse(this.stateMachine.IsCardPlayable());
        }

        [Test]
        public void Start_FallsBackToFirstWaitingCard_WhenNoneInStartingSuit()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Club", "02"), ("Club", "05"), ("Club", "09"),
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"), ("Club", "13"));
            var state = new ComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            state.Start();

            Assert.AreEqual(Card.CardState.Played, hand.GetHand()[0].state);
        }

        [Test]
        public void Start_WhenCardNotPlayable_Throws()
        {
            this.stateMachine.SetCardPlayable(false);
            var hand = HandTestUtils.BuildUniformHand(this.createdCards, this.stateMachine, "Club", "02");
            var state = new ComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            Assert.Throws<Exception>(() => state.Start());
        }
    }
}
