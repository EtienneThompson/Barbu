namespace Barbu.Tests.EditMode
{
    using System.Collections.Generic;
    using System.Linq;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;
    using UnityEngine;

    public class HandTests
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

        // Builds a fixed, deterministic 13-card hand. Hand allocates a
        // fixed-size 13-slot array, so a partially filled hand leaves null
        // slots that most Hand methods will throw on - always deal a full hand.
        private Hand CreateStandardHand()
        {
            var hand = new Hand();
            var specs = new (string suit, string rank)[]
            {
                ("Heart", "02"), ("Heart", "05"), ("Heart", "09"), ("Heart", "01"), // Ace -> rank 14
                ("Diamond", "03"), ("Diamond", "10"),
                ("Spade", "04"), ("Spade", "07"), ("Spade", "11"), ("Spade", "13"),
                ("Club", "06"), ("Club", "08"), ("Club", "12"),
            };

            foreach (var (suit, rank) in specs)
            {
                var card = CardTestUtils.CreateCard(suit, rank);
                this.createdCards.Add(card);
                hand.AddCard(card);
            }

            return hand;
        }

        [Test]
        public void CardsInSuit_ReturnsOnlyMatchingSuit()
        {
            var hand = this.CreateStandardHand();

            var hearts = hand.CardsInSuit("Heart");

            Assert.AreEqual(4, hearts.Count);
            Assert.IsTrue(hearts.All(c => c.suit == "Heart"));
        }

        [Test]
        public void CardsInSuit_ExcludesPlayedCards()
        {
            var hand = this.CreateStandardHand();
            hand.GetHand()[0].state = Card.CardState.Played; // Heart2

            var hearts = hand.CardsInSuit("Heart");

            Assert.AreEqual(3, hearts.Count);
        }

        [Test]
        public void GetHighestCard_ReturnsHighestRankedWaitingCard()
        {
            var hand = this.CreateStandardHand();

            var highest = hand.GetHighestCard();

            Assert.AreEqual("Heart", highest.suit);
            Assert.AreEqual(14, highest.rank); // Ace
        }

        [Test]
        public void GetHighestCard_ExcludesPlayedCardsByDefault()
        {
            var hand = this.CreateStandardHand();
            var ace = hand.GetHand().First(c => c.rank == 14);
            ace.state = Card.CardState.Played;

            var highest = hand.GetHighestCard();

            Assert.AreEqual(13, highest.rank); // Spade King, next highest
        }

        [Test]
        public void GetHighestCard_IncludePlayedTrue_ConsidersPlayedCards()
        {
            var hand = this.CreateStandardHand();
            var ace = hand.GetHand().First(c => c.rank == 14);
            ace.state = Card.CardState.Played;

            var highest = hand.GetHighestCard(includePlayed: true);

            Assert.AreEqual(14, highest.rank);
        }

        [Test]
        public void GetLowestCard_ReturnsLowestRankedWaitingCard()
        {
            var hand = this.CreateStandardHand();

            var lowest = hand.GetLowestCard();

            Assert.AreEqual("Heart", lowest.suit);
            Assert.AreEqual(2, lowest.rank);
        }

        [Test]
        public void GetAvailableCards_ExcludesOnlyPlayedCards()
        {
            var hand = this.CreateStandardHand();
            hand.GetHand()[0].state = Card.CardState.Played;

            var available = hand.GetAvailableCards();

            Assert.AreEqual(12, available.Count);
        }

        [Test]
        public void GetCardsWithPoints_ReturnsOnlyCardsThatScoreForTheRound()
        {
            var hand = this.CreateStandardHand();
            var heartsRound = new HeartsRound();

            var pointCards = hand.GetCardsWithPoints(heartsRound);

            Assert.AreEqual(4, pointCards.Count);
            Assert.IsTrue(pointCards.All(c => c.suit == "Heart"));
        }

        [Test]
        public void SortHand_LowToHigh_OrdersByAscendingRank()
        {
            var hand = this.CreateStandardHand();

            hand.SortHand(Settings.SortingOptions.LowToHigh);

            var ranks = hand.GetHand().Select(c => c.rank).ToArray();
            var sortedRanks = ranks.OrderBy(r => r).ToArray();
            CollectionAssert.AreEqual(sortedRanks, ranks);
        }
    }
}
