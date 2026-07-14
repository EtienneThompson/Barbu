namespace Barbu.Tests.EditMode
{
    using System.Collections.Generic;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class RoundTests
    {
        private readonly List<Card> createdCards = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var card in this.createdCards)
            {
                UnityEngine.Object.DestroyImmediate(card.gameObject);
            }

            this.createdCards.Clear();
        }

        private Pile CreatePileWith(params (string suit, string rank)[] cardSpecs)
        {
            var pile = new Pile(new FakeStateMachine(), new FakeEventsController());
            foreach (var (suit, rank) in cardSpecs)
            {
                var card = CardTestUtils.CreateCard(suit, rank);
                this.createdCards.Add(card);
                pile.AddCardToPile(card);
            }

            return pile;
        }

        [Test]
        public void CalculatePointsInPile_OnlyCountsPointEarningCards()
        {
            var round = new HeartsRound();
            // 3 hearts (5 pts each) + 1 non-scoring club.
            var pile = this.CreatePileWith(("Heart", "02"), ("Heart", "05"), ("Heart", "09"), ("Club", "04"));

            var points = round.CalculatePointsInPile(pile);

            Assert.AreEqual(15, points);
        }

        [Test]
        public void CalculatePointsInPile_AddsPointsPerPile()
        {
            var round = new PilesRound(); // PointsPerPile = 5, no card scoring.
            var pile = this.CreatePileWith(("Heart", "02"), ("Club", "04"));

            var points = round.CalculatePointsInPile(pile);

            Assert.AreEqual(5, points);
        }

        [Test]
        public void CalculatePointsInAllPiles_SumsAcrossPiles()
        {
            var round = new HeartsRound();
            var pileA = this.CreatePileWith(("Heart", "02"), ("Club", "04"));
            var pileB = this.CreatePileWith(("Heart", "05"), ("Heart", "09"));

            var points = round.CalculatePointsInAllPiles(new List<Pile> { pileA, pileB });

            Assert.AreEqual(15, points);
        }

        [Test]
        public void IsPointEarningCard_KnownAndUnknownCards()
        {
            var round = new HeartsRound();

            Assert.IsTrue(round.IsPointEarningCard("Heart5"));
            Assert.IsFalse(round.IsPointEarningCard("Club5"));
        }

        [Test]
        public void GetCardPointValue_UnknownCard_ReturnsZero()
        {
            var round = new HeartsRound();

            Assert.AreEqual(0, round.GetCardPointValue("Club5"));
            Assert.AreEqual(5, round.GetCardPointValue("Heart5"));
        }

        [Test]
        public void IsRoundOver_TrueOnlyWhenPointsMatchTotal()
        {
            var round = new HeartsRound(); // TotalPoints = 65
            var pointsShortOfTotal = new Dictionary<string, int[]>
            {
                { "1", new[] { 30 } },
                { "2", new[] { 30 } },
            };
            var pointsAtTotal = new Dictionary<string, int[]>
            {
                { "1", new[] { 35 } },
                { "2", new[] { 30 } },
            };

            Assert.IsFalse(round.IsRoundOver(0, pointsShortOfTotal, playedPiles: 10));
            Assert.IsTrue(round.IsRoundOver(0, pointsAtTotal, playedPiles: 13));
        }

        [Test]
        public void IsRoundPositive_ReflectsSignOfTotalPoints()
        {
            Assert.IsFalse(new HeartsRound().IsRoundPositive()); // TotalPoints = 65
            Assert.IsTrue(new EverythingRound().IsRoundPositive()); // TotalPoints = -210
        }
    }
}
