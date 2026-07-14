namespace Barbu.Tests.EditMode
{
    using System;
    using System.Collections.Generic;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;
    using UnityEngine;

    public class DeckTests
    {
        private FakeCardFactory cardFactory;
        private Deck deck;

        [SetUp]
        public void SetUp()
        {
            this.cardFactory = new FakeCardFactory();
            this.deck = new Deck(this.cardFactory);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var card in this.cardFactory.CreatedCards)
            {
                if (card != null)
                {
                    UnityEngine.Object.DestroyImmediate(card.gameObject);
                }
            }
        }

        [Test]
        public void ResetDeck_FillsDeckWithAllCards()
        {
            Assert.AreEqual(Constants.CardsInDeck, this.deck.CardsInDeck);
        }

        [Test]
        public void DrawCard_RemovesOneCardFromDeck()
        {
            this.deck.DrawCard("1");

            Assert.AreEqual(Constants.CardsInDeck - 1, this.deck.CardsInDeck);
        }

        [Test]
        public void DrawCard_WithNullOrEmptyPlayerId_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.deck.DrawCard(null));
            Assert.Throws<ArgumentNullException>(() => this.deck.DrawCard(string.Empty));
        }

        [Test]
        public void DrawCard_PastEndOfDeck_ThrowsInvalidOperationException()
        {
            this.deck.DrawCards("1", Constants.CardsInDeck);

            Assert.Throws<InvalidOperationException>(() => this.deck.DrawCard("1"));
        }

        [Test]
        public void TryDrawCard_PastEndOfDeck_ReturnsFalseInsteadOfThrowing()
        {
            this.deck.DrawCards("1", Constants.CardsInDeck);

            var result = this.deck.TryDrawCard("1", out var card);

            Assert.IsFalse(result);
            Assert.IsNull(card);
        }

        [Test]
        public void DrawCards_WithNegativeCount_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => this.deck.DrawCards("1", -1));
        }

        [Test]
        public void DrawCards_DrawsEveryCardExactlyOnce()
        {
            var drawn = this.deck.DrawCards("1", Constants.CardsInDeck);

            var uniqueCards = new HashSet<string>();
            foreach (var card in drawn)
            {
                uniqueCards.Add(card.GetName());
            }

            Assert.AreEqual(Constants.CardsInDeck, drawn.Count);
            Assert.AreEqual(Constants.CardsInDeck, uniqueCards.Count);
            Assert.AreEqual(0, this.deck.CardsInDeck);
        }

        [Test]
        public void ShuffleComplete_PreservesCardCount()
        {
            this.deck.ShuffleComplete();

            Assert.AreEqual(Constants.CardsInDeck, this.deck.CardsInDeck);
        }

        [Test]
        public void ResetDeck_AfterDrawingCards_RestoresFullDeck()
        {
            this.deck.DrawCards("1", 10);

            this.deck.ResetDeck();

            Assert.AreEqual(Constants.CardsInDeck, this.deck.CardsInDeck);
        }
    }
}
