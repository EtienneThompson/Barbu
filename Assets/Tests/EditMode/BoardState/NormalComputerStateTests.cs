namespace Barbu.Tests.EditMode.BoardState
{
    using System;
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class NormalComputerStateTests
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

        private Hand BuildHeartHeavyHand()
        {
            this.stateMachine.SetStartingSuit("Heart");
            return HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "02"), ("Heart", "09"), ("Heart", "01"), // Ace -> rank 14
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"), ("Club", "13"));
        }

        [Test]
        public void Start_RoundIsPositive_PlaysHighestCardInStartingSuit()
        {
            this.stateMachine.SetCardPlayable(true);
            var hand = this.BuildHeartHeavyHand();
            var state = new NormalComputerState(this.stateMachine, new FakeTelemetryService(), new EverythingRound(), "1", hand);

            state.Start();

            var ace = hand.GetHand()[2];
            Assert.AreEqual(14, ace.rank);
            Assert.AreEqual(Card.CardState.Played, ace.state);
            Assert.IsFalse(this.stateMachine.IsCardPlayable());
        }

        [Test]
        public void Start_RoundIsNotPositive_PlaysLowestCardInStartingSuit()
        {
            this.stateMachine.SetCardPlayable(true);
            var hand = this.BuildHeartHeavyHand();
            var state = new NormalComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            state.Start();

            var lowestHeart = hand.GetHand()[0];
            Assert.AreEqual(2, lowestHeart.rank);
            Assert.AreEqual(Card.CardState.Played, lowestHeart.state);
        }

        [Test]
        public void Start_NoCardsInStartingSuit_ConsidersAllAvailableCards()
        {
            this.stateMachine.SetStartingSuit("Heart"); // hand below has no hearts
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Club", "02"), ("Club", "09"),
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"), ("Club", "13"), ("Club", "05"));
            var state = new NormalComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand); // not positive -> lowest

            state.Start();

            var lowestOverall = hand.GetHand()[0]; // Club2
            Assert.AreEqual(2, lowestOverall.rank);
            Assert.AreEqual(Card.CardState.Played, lowestOverall.state);
        }

        [Test]
        public void Start_WhenCardNotPlayable_Throws()
        {
            this.stateMachine.SetCardPlayable(false);
            var hand = HandTestUtils.BuildUniformHand(this.createdCards, this.stateMachine, "Club", "02");
            var state = new NormalComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            Assert.Throws<Exception>(() => state.Start());
        }
    }
}
