namespace Barbu.Tests.EditMode.BoardState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    // HardComputerState.Start() has many branches (single card in suit, leading
    // vs. following, point-earning fallback, round sign). These tests cover the
    // major branches rather than every rank/tie-break permutation.
    public class HardComputerStateTests
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

        private static Card Find(Hand hand, string suit, int rank) =>
            hand.GetHand().First(c => c.suit == suit && c.rank == rank);

        [Test]
        public void Start_OnlyOneCardInStartingSuit_PlaysItRegardlessOfRank()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "09"),
                ("Club", "02"), ("Club", "03"), ("Club", "04"), ("Club", "05"),
                ("Club", "06"), ("Club", "07"), ("Club", "08"), ("Club", "10"),
                ("Club", "11"), ("Club", "13"), ("Diamond", "02"), ("Diamond", "03"));
            var state = new HardComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            state.Start();

            Assert.AreEqual(Card.CardState.Played, Find(hand, "Heart", 9).state);
        }

        [Test]
        public void Start_GoingFirst_RoundNotPositive_PlaysLowestCardInSuit()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "02"), ("Heart", "09"), ("Heart", "01"), // Ace -> 14
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"), ("Club", "13"));
            var state = new HardComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            state.Start(); // NumCardsPlayed() defaults to 0 -> "going first"

            Assert.AreEqual(Card.CardState.Played, Find(hand, "Heart", 2).state);
        }

        [Test]
        public void Start_GoingFirst_RoundPositive_PlaysHighestCardInSuit()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "02"), ("Heart", "09"), ("Heart", "01"),
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"), ("Club", "13"));
            var state = new HardComputerState(this.stateMachine, new FakeTelemetryService(), new EverythingRound(), "1", hand);

            state.Start();

            Assert.AreEqual(Card.CardState.Played, Find(hand, "Heart", 14).state);
        }

        [Test]
        public void Start_NoCardsInSuitAndNoPointCards_PlaysHighestCardOverall()
        {
            this.stateMachine.SetStartingSuit("Heart"); // no hearts in hand below
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Club", "02"), ("Club", "09"),
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "05"), ("Club", "10"), ("Club", "04"));
            // PilesRound has an empty PointMapping and is not positive (TotalPoints = 65).
            var state = new HardComputerState(this.stateMachine, new FakeTelemetryService(), new PilesRound(), "1", hand);

            state.Start();

            Assert.AreEqual(Card.CardState.Played, Find(hand, "Spade", 13).state); // highest rank in hand
        }

        [Test]
        public void Start_NoCardsInSuitButHasPointCards_PicksHighestValueThenTieBreaksByEarlierHandPosition()
        {
            this.stateMachine.SetStartingSuit("Heart"); // no hearts in hand below
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Spade", "12"), ("Club", "12"), // both worth 10 in NothingRound; Spade12 comes first
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "05"), ("Club", "10"), ("Club", "04"));
            var state = new HardComputerState(this.stateMachine, new FakeTelemetryService(), new NothingRound(), "1", hand);

            state.Start();

            Assert.AreEqual(Card.CardState.Played, Find(hand, "Spade", 12).state);
            Assert.AreEqual(Card.CardState.Waiting, Find(hand, "Club", 12).state);
        }

        [Test]
        public void Start_SecondToPlay_RoundNotPositive_PlaysHighestCardBelowLeadingRank()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            this.stateMachine.IncrementNumCardsPlayed(); // NumCardsPlayed() == 1 -> "second to play"
            this.stateMachine.SetHighestRank(10);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "05"), ("Heart", "08"), ("Heart", "12"),
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "09"), ("Club", "02"), ("Club", "03"));
            var state = new HardComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            state.Start();

            Assert.AreEqual(Card.CardState.Played, Find(hand, "Heart", 8).state); // highest heart still below rank 10
        }

        [Test]
        public void Start_WhenCardNotPlayable_Throws()
        {
            this.stateMachine.SetCardPlayable(false);
            var hand = HandTestUtils.BuildUniformHand(this.createdCards, this.stateMachine, "Club", "02");
            var state = new HardComputerState(this.stateMachine, new FakeTelemetryService(), new HeartsRound(), "1", hand);

            Assert.Throws<Exception>(() => state.Start());
        }
    }
}
