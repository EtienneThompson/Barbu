namespace Barbu.Tests.EditMode.BoardState
{
    using System;
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    // EasyComputerState.Start() is currently identical to ComputerState.Start() -
    // same "first waiting card in suit, else first waiting card" logic. Tested
    // separately since it's a distinct concrete class the factory can select.
    public class EasyComputerStateTests
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
            this.stateMachine.SetStartingSuit("Spade");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Club", "02"), ("Heart", "05"), ("Heart", "09"),
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"), ("Club", "13"));
            var state = new EasyComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            state.Start();

            var cards = hand.GetHand();
            Assert.AreEqual(Card.CardState.Played, cards[5].state); // Spade4, first in suit
            Assert.AreEqual(Card.CardState.Waiting, cards[6].state);
            Assert.AreEqual(1, this.stateMachine.NumCardsPlayed());
            Assert.IsFalse(this.stateMachine.IsCardPlayable());
        }

        [Test]
        public void Start_FallsBackToFirstWaitingCard_WhenNoneInStartingSuit()
        {
            this.stateMachine.SetStartingSuit("Diamond");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildUniformHand(this.createdCards, this.stateMachine, "Club", "02");
            var state = new EasyComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            state.Start();

            Assert.AreEqual(Card.CardState.Played, hand.GetHand()[0].state);
        }

        [Test]
        public void Start_WhenCardNotPlayable_Throws()
        {
            this.stateMachine.SetCardPlayable(false);
            var hand = HandTestUtils.BuildUniformHand(this.createdCards, this.stateMachine, "Club", "02");
            var state = new EasyComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            Assert.Throws<Exception>(() => state.Start());
        }
    }
}
