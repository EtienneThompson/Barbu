namespace Barbu.Tests.EditMode.BoardState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Barbu.Gameplay;
    using Barbu.Gameplay.BoardState;
    using Barbu.Gameplay.Rounds;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    /// <summary>
    /// Covers every branch of HardComputerState.Start(): the playable guard, the
    /// void-in-suit path (with and without point-earning cards, in both round
    /// signs), the single-card-in-suit shortcut, and the turn-order cases
    /// (leading, 2nd, 3rd, last) crossed with the fallbacks inside
    /// Hand.GetCardAboveRank / Hand.GetCardBelowRank.
    /// Rounds are chosen for their sign and point mapping: only EverythingRound
    /// is positive (TotalPoints -210); HeartsRound (every heart worth 5),
    /// NothingRound (weighted mapping), KingOfHeartsRound (a single point card)
    /// and PilesRound (empty mapping) are all not positive.
    /// </summary>
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

        private static List<Card> PlayedCards(Hand hand) =>
            hand.GetHand().Where(c => c.state == Card.CardState.Played).ToList();

        /// <summary>
        /// Asserts the given card was played and that Start() played exactly one
        /// card on top of however many the test had already marked as played.
        /// </summary>
        private static void AssertPlayed(Hand hand, string suit, int rank, int previouslyPlayed = 0)
        {
            Assert.AreEqual(Card.CardState.Played, Find(hand, suit, rank).state, $"expected {suit}{rank} to be played");
            Assert.AreEqual(
                previouslyPlayed + 1,
                PlayedCards(hand).Count,
                "Start() should play exactly one card: " + string.Join(", ", PlayedCards(hand).Select(c => c.GetName())));
        }

        private HardComputerState CreateState(Hand hand, IRound round) =>
            new HardComputerState(this.stateMachine, new FakeTelemetryService(), round, "1", hand);

        private void SetNumCardsPlayed(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.stateMachine.IncrementNumCardsPlayed();
            }
        }

        /// <summary>
        /// Builds a full 13-card hand holding exactly the requested hearts, in the
        /// order given, padded out with clubs and then diamonds. Tests that only
        /// care about the starting suit use this so Hand's fixed 13-slot array
        /// never has null slots. The padding deliberately skips queens so it never
        /// adds a point-earning card to the rounds these tests use.
        /// </summary>
        private Hand BuildHandWithHearts(params string[] heartRanks)
        {
            var specs = new List<(string suit, string rank)>();
            foreach (var rank in heartRanks)
            {
                specs.Add(("Heart", rank));
            }

            var fillerRanks = new[] { "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "13" };
            foreach (var suit in new[] { "Club", "Diamond" })
            {
                foreach (var rank in fillerRanks)
                {
                    if (specs.Count == 13)
                    {
                        break;
                    }

                    specs.Add((suit, rank));
                }
            }

            Assert.AreEqual(13, specs.Count, "hands must be full - Hand allocates a fixed 13-slot array");
            return HandTestUtils.BuildHand(this.createdCards, this.stateMachine, specs.ToArray());
        }

        // ---------------------------------------------------------------------
        // Guard and per-turn side effects.
        // ---------------------------------------------------------------------

        [Test]
        public void Start_WhenCardNotPlayable_ThrowsAndPlaysNothing()
        {
            this.stateMachine.SetCardPlayable(false);
            var hand = HandTestUtils.BuildUniformHand(this.createdCards, this.stateMachine, "Club", "02");
            var state = this.CreateState(hand, new HeartsRound());

            Assert.Throws<Exception>(() => state.Start());

            CollectionAssert.IsEmpty(PlayedCards(hand));
        }

        [Test]
        public void Start_ClearsPlayableFlagAndCountsTheCardPlayed()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = this.BuildHandWithHearts("05", "08", "12");
            var state = this.CreateState(hand, new HeartsRound());

            state.Start();

            Assert.IsFalse(this.stateMachine.IsCardPlayable());
            Assert.AreEqual(1, this.stateMachine.NumCardsPlayed());
        }

        // ---------------------------------------------------------------------
        // Exactly one card in the starting suit: play it, whatever else is true.
        // (This branch is only reachable via cardsInSuit, since an empty
        // cardsInSuit takes the void path before playableCards is consulted.)
        // ---------------------------------------------------------------------

        [Test]
        public void Start_OnlyOneCardInStartingSuit_RoundNotPositive_PlaysItRegardlessOfRank()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = this.BuildHandWithHearts("09");
            var state = this.CreateState(hand, new HeartsRound());

            state.Start();

            AssertPlayed(hand, "Heart", 9);
        }

        [Test]
        public void Start_OnlyOneCardInStartingSuit_RoundPositive_PlaysItRegardlessOfRank()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = this.BuildHandWithHearts("09");
            var state = this.CreateState(hand, new EverythingRound());

            state.Start();

            AssertPlayed(hand, "Heart", 9);
        }

        [Test]
        public void Start_OnlyOneCardInStartingSuit_PlaysItEvenWhenFollowingAnUnbeatableCard()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            this.SetNumCardsPlayed(3); // going last, and the leading Ace can't be beaten
            this.stateMachine.SetHighestRank(14);
            var hand = this.BuildHandWithHearts("09");
            var state = this.CreateState(hand, new EverythingRound());

            state.Start();

            AssertPlayed(hand, "Heart", 9);
        }

        // ---------------------------------------------------------------------
        // Leading the pile (NumCardsPlayed() == 0).
        // ---------------------------------------------------------------------

        [Test]
        public void Start_Leading_RoundNotPositive_PlaysLowestCardInSuit()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = this.BuildHandWithHearts("02", "09", "01"); // Ace -> rank 14
            var state = this.CreateState(hand, new HeartsRound());

            state.Start();

            AssertPlayed(hand, "Heart", 2);
        }

        [Test]
        public void Start_Leading_RoundPositive_PlaysHighestCardInSuit()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = this.BuildHandWithHearts("02", "09", "01");
            var state = this.CreateState(hand, new EverythingRound());

            state.Start();

            AssertPlayed(hand, "Heart", 14);
        }

        [Test]
        public void Start_Leading_IgnoresCardsAlreadyPlayed()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = this.BuildHandWithHearts("05", "08", "12");
            Find(hand, "Heart", 5).state = Card.CardState.Played;
            var state = this.CreateState(hand, new HeartsRound());

            state.Start();

            AssertPlayed(hand, "Heart", 8, previouslyPlayed: 1); // next lowest heart still in hand
        }

        [Test]
        public void Start_LeadingWithNoStartingSuitSet_TreatsWholeHandAsPlayable()
        {
            // The starting suit is empty until someone leads, and CardsInSuit("")
            // returns every unplayed card, so the playable set is the whole hand.
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "05"), ("Heart", "09"),
                ("Diamond", "02"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"), ("Club", "13"), ("Club", "03"));
            var state = this.CreateState(hand, new HeartsRound()); // not positive -> lowest

            state.Start();

            AssertPlayed(hand, "Diamond", 2); // lowest card in the hand, suit ignored
        }

        // ---------------------------------------------------------------------
        // Following the pile. Hearts 5/8/12 are the playable set in every case;
        // 2nd and 3rd share the useLowestFallback: true path, going last uses
        // useLowestFallback: false.
        // ---------------------------------------------------------------------

        [TestCase(1, 10, 8)]  // 2nd: highest heart below the leading rank
        [TestCase(2, 10, 8)]  // 3rd: same rule
        [TestCase(3, 10, 8)]  // last: same rule
        [TestCase(1, 8, 5)]   // "below" is strict, so the equal-ranked heart is skipped
        [TestCase(2, 8, 5)]
        [TestCase(3, 8, 5)]
        [TestCase(1, 4, 5)]   // nothing below: 2nd/3rd dump the lowest heart
        [TestCase(2, 4, 5)]
        [TestCase(3, 4, 12)]  // nothing below going last: take the pile with the highest heart
        [TestCase(4, 4, 12)]  // defensive: any count past 3 is also treated as going last
        public void Start_Following_RoundNotPositive_DucksUnderTheLeadingRank(int cardsPlayed, int leadingRank, int expectedRank)
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            this.SetNumCardsPlayed(cardsPlayed);
            this.stateMachine.SetHighestRank(leadingRank);
            var hand = this.BuildHandWithHearts("05", "08", "12");
            var state = this.CreateState(hand, new HeartsRound());

            state.Start();

            AssertPlayed(hand, "Heart", expectedRank);
        }

        [TestCase(1, 10, 12)] // 2nd: highest heart beats the leading rank
        [TestCase(2, 10, 12)] // 3rd: same rule
        [TestCase(3, 10, 12)] // last: same rule
        [TestCase(1, 12, 12)] // matching the leading rank still counts as "above"
        [TestCase(2, 12, 12)]
        [TestCase(3, 12, 12)]
        [TestCase(1, 13, 5)]  // can't beat it, so throw away the lowest heart
        [TestCase(2, 13, 5)]
        [TestCase(3, 13, 5)]
        [TestCase(4, 13, 5)]  // defensive: any count past 3 is also treated as going last
        public void Start_Following_RoundPositive_TriesToTakeThePile(int cardsPlayed, int leadingRank, int expectedRank)
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            this.SetNumCardsPlayed(cardsPlayed);
            this.stateMachine.SetHighestRank(leadingRank);
            var hand = this.BuildHandWithHearts("05", "08", "12");
            var state = this.CreateState(hand, new EverythingRound());

            state.Start();

            AssertPlayed(hand, "Heart", expectedRank);
        }

        // ---------------------------------------------------------------------
        // Void in the starting suit, no point-earning cards in hand.
        // ---------------------------------------------------------------------

        [Test]
        public void Start_VoidInSuitAndNoPointCards_RoundNotPositive_PlaysHighestCardOverall()
        {
            this.stateMachine.SetStartingSuit("Heart"); // no hearts in the hand below
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Club", "02"), ("Club", "09"),
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "05"), ("Club", "10"), ("Club", "04"));
            var state = this.CreateState(hand, new PilesRound()); // empty mapping, not positive

            state.Start();

            AssertPlayed(hand, "Spade", 13);
        }

        [Test]
        public void Start_VoidInSuitAndNoPointCards_RoundPositive_PlaysLowestCardOverall()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            // EverythingRound scores every heart and every queen, so this hand has
            // neither and takes the "no point earning cards" branch.
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Club", "02"), ("Club", "05"), ("Club", "07"), ("Club", "09"), ("Club", "13"),
                ("Diamond", "03"), ("Diamond", "06"), ("Diamond", "08"), ("Diamond", "10"), ("Diamond", "13"),
                ("Spade", "04"), ("Spade", "11"), ("Spade", "01")); // Ace -> rank 14
            var state = this.CreateState(hand, new EverythingRound());

            state.Start();

            AssertPlayed(hand, "Club", 2);
        }

        [Test]
        public void Start_AllCardsInStartingSuitAlreadyPlayed_UsesVoidPath()
        {
            this.stateMachine.SetStartingSuit("Heart");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "03"), ("Heart", "01"), // Ace -> rank 14, the highest card in hand
                ("Diamond", "06"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "02"), ("Club", "05"), ("Club", "08"), ("Club", "09"), ("Club", "10"));
            Find(hand, "Heart", 3).state = Card.CardState.Played;
            Find(hand, "Heart", 14).state = Card.CardState.Played;
            var state = this.CreateState(hand, new PilesRound());

            state.Start();

            // Highest card still in hand; the played Ace of hearts is skipped.
            AssertPlayed(hand, "Spade", 13, previouslyPlayed: 2);
        }

        // ---------------------------------------------------------------------
        // Void in the starting suit, holding point-earning cards.
        // ---------------------------------------------------------------------

        [Test]
        public void Start_VoidInSuitWithSinglePointCard_PlaysThatCard()
        {
            this.stateMachine.SetStartingSuit("Spade"); // no spades in the hand below
            this.stateMachine.SetCardPlayable(true);
            // KingOfHeartsRound scores only Heart13, which is not the highest card
            // in hand - so this can only be the point-card branch.
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "13"), ("Heart", "04"),
                ("Diamond", "01"), ("Diamond", "06"), ("Diamond", "10"), // Diamond Ace -> rank 14
                ("Club", "02"), ("Club", "03"), ("Club", "05"), ("Club", "07"),
                ("Club", "08"), ("Club", "09"), ("Club", "11"), ("Club", "12"));
            var state = this.CreateState(hand, new KingOfHeartsRound());

            state.Start();

            AssertPlayed(hand, "Heart", 13);
        }

        [Test]
        public void Start_VoidInSuitWithPointCards_RoundNotPositive_PlaysHighestValuePointCard()
        {
            this.stateMachine.SetStartingSuit("Spade");
            this.stateMachine.SetCardPlayable(true);
            // NothingRound: Heart2 is worth 5, Club12 10, Heart13 45.
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "02"), ("Heart", "13"), ("Club", "12"),
                ("Diamond", "03"), ("Diamond", "06"), ("Diamond", "10"),
                ("Club", "03"), ("Club", "05"), ("Club", "07"),
                ("Club", "08"), ("Club", "09"), ("Club", "11"), ("Club", "13"));
            var state = this.CreateState(hand, new NothingRound());

            state.Start();

            AssertPlayed(hand, "Heart", 13);
        }

        [Test]
        public void Start_VoidInSuitWithEquallyValuedPointCards_RoundNotPositive_PlaysHighestRanked()
        {
            this.stateMachine.SetStartingSuit("Spade");
            this.stateMachine.SetCardPlayable(true);
            // HeartsRound scores every heart at 5, so only rank breaks the tie.
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "03"), ("Heart", "09"), ("Heart", "11"),
                ("Diamond", "03"), ("Diamond", "06"), ("Diamond", "10"),
                ("Club", "02"), ("Club", "05"), ("Club", "07"),
                ("Club", "08"), ("Club", "09"), ("Club", "11"), ("Club", "12"));
            var state = this.CreateState(hand, new HeartsRound());

            state.Start();

            AssertPlayed(hand, "Heart", 11);
        }

        [Test]
        public void Start_VoidInSuitWithPointCards_RoundNotPositive_KeepsFirstCandidateWhenHigherValueIsLowerRanked()
        {
            this.stateMachine.SetStartingSuit("Spade");
            this.stateMachine.SetCardPlayable(true);
            // The selection loop only moves to a card that is worth at least as
            // much AND ranks higher than the current pick, so a higher-value card
            // that ranks lower than an earlier candidate is passed over: Heart14
            // (5 points) is picked before Heart13 (45 points) is considered.
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "01"), ("Heart", "13"), // Ace -> rank 14, then the King
                ("Diamond", "03"), ("Diamond", "06"), ("Diamond", "10"),
                ("Club", "02"), ("Club", "03"), ("Club", "05"), ("Club", "07"),
                ("Club", "08"), ("Club", "09"), ("Club", "11"), ("Club", "13"));
            var state = this.CreateState(hand, new NothingRound());

            state.Start();

            AssertPlayed(hand, "Heart", 14);
        }

        [Test]
        public void Start_VoidInSuitWithPointCards_IgnoresPointCardsAlreadyPlayed()
        {
            this.stateMachine.SetStartingSuit("Spade");
            this.stateMachine.SetCardPlayable(true);
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "13"), ("Heart", "02"), // King is worth 45 in NothingRound
                ("Diamond", "03"), ("Diamond", "06"), ("Diamond", "10"),
                ("Club", "02"), ("Club", "03"), ("Club", "05"), ("Club", "07"),
                ("Club", "08"), ("Club", "09"), ("Club", "11"), ("Club", "13"));
            Find(hand, "Heart", 13).state = Card.CardState.Played;
            var state = this.CreateState(hand, new NothingRound());

            state.Start();

            AssertPlayed(hand, "Heart", 2, previouslyPlayed: 1);
        }

        [Test]
        public void Start_VoidInSuitWithPointCards_RoundPositive_HoldsPointCardsAndPlaysLowestCardWithoutPoints()
        {
            this.stateMachine.SetStartingSuit("Spade");
            this.stateMachine.SetCardPlayable(true);
            // EverythingRound: Heart13 is worth -45 and Heart5 -5. The computer
            // can't win this pile, so discarding either one would just hand those
            // points to whoever does.
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "13"), ("Heart", "05"),
                ("Diamond", "03"), ("Diamond", "06"), ("Diamond", "10"),
                ("Club", "02"), ("Club", "03"), ("Club", "05"), ("Club", "07"),
                ("Club", "08"), ("Club", "09"), ("Club", "11"), ("Club", "13"));
            var state = this.CreateState(hand, new EverythingRound());

            state.Start();

            AssertPlayed(hand, "Club", 2); // lowest card that scores nothing
            Assert.AreEqual(Card.CardState.Waiting, Find(hand, "Heart", 13).state);
            Assert.AreEqual(Card.CardState.Waiting, Find(hand, "Heart", 5).state);
        }

        [Test]
        public void Start_VoidInSuit_RoundPositive_HoldsPointCardEvenWhenItIsTheLowestCardInHand()
        {
            this.stateMachine.SetStartingSuit("Spade");
            this.stateMachine.SetCardPlayable(true);
            // Heart2 is the lowest card in the hand, but it scores -5, so the lowest
            // card that scores nothing goes instead.
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "02"),
                ("Club", "03"), ("Club", "05"), ("Club", "07"), ("Club", "08"),
                ("Club", "09"), ("Club", "11"), ("Club", "13"),
                ("Diamond", "04"), ("Diamond", "06"), ("Diamond", "10"), ("Diamond", "11"), ("Diamond", "13"));
            var state = this.CreateState(hand, new EverythingRound());

            state.Start();

            AssertPlayed(hand, "Club", 3);
            Assert.AreEqual(Card.CardState.Waiting, Find(hand, "Heart", 2).state);
        }

        [Test]
        public void Start_VoidInSuitHoldingOnlyPointCards_RoundPositive_GivesUpCheapestPointCard()
        {
            this.stateMachine.SetStartingSuit("Spade");
            this.stateMachine.SetCardPlayable(true);
            // Every heart scores in EverythingRound, so with a hand of nothing but
            // hearts the computer has to give one up. Heart12 (-15) and Heart13
            // (-45) are the expensive ones; the rest are worth -5 apiece, and among
            // those it keeps the high cards that can still win piles.
            var hand = HandTestUtils.BuildHand(
                this.createdCards,
                this.stateMachine,
                ("Heart", "02"), ("Heart", "03"), ("Heart", "04"), ("Heart", "05"),
                ("Heart", "06"), ("Heart", "07"), ("Heart", "08"), ("Heart", "09"),
                ("Heart", "10"), ("Heart", "11"), ("Heart", "12"), ("Heart", "13"),
                ("Heart", "01")); // Ace -> rank 14
            var state = this.CreateState(hand, new EverythingRound());

            state.Start();

            AssertPlayed(hand, "Heart", 2);
            Assert.AreEqual(Card.CardState.Waiting, Find(hand, "Heart", 13).state);
            Assert.AreEqual(Card.CardState.Waiting, Find(hand, "Heart", 12).state);
        }
    }
}
